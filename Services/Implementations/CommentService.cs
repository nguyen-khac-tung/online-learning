using Microsoft.EntityFrameworkCore;
using Online_Learning.Constants;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Comment;
using Online_Learning.Models.DTOs.Common;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using System.Linq;



namespace Online_Learning.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly OnlineLearningContext _context;
        private readonly ILogger<CommentService> _logger;

        public CommentService(OnlineLearningContext context, ILogger<CommentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<CommentDto>>> GetCommentsAsync(CommentFilterRequest request)
        {
            try
            {
                var query = _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Lesson)
                    //.Include(c => c.InverseParentComment)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(c => c.Content!.Contains(request.SearchTerm) ||
                                           c.User.FullName.Contains(request.SearchTerm));
                }

                if (request.LessonId.HasValue)
                {
                    query = query.Where(c => c.LessonId == request.LessonId);
                }

                if (!string.IsNullOrEmpty(request.UserId))
                {
                    query = query.Where(c => c.UserId == request.UserId);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(c => c.Status == request.Status);
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt >= request.FromDate);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt <= request.ToDate);
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "createdat" => request.IsDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    "updatedat" => request.IsDescending ? query.OrderByDescending(c => c.UpdatedAt) : query.OrderBy(c => c.UpdatedAt),
                    "username" => request.IsDescending ? query.OrderByDescending(c => c.User.FullName) : query.OrderBy(c => c.User.FullName),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                var comments = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var commentDtos = comments.Select(c => MapToDto(c, true)).ToList();

                var result = new PagedResult<CommentDto>
                {
                    Data = commentDtos,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    HasNext = request.Page < totalPages,
                    HasPrevious = request.Page > 1
                };

                return ApiResponse<PagedResult<CommentDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting comments");
                return ApiResponse<PagedResult<CommentDto>>.ErrorResponse(Messages.ErrorRetrievingComments);
            }
        }

        public async Task<ApiResponse<CommentDto>> GetCommentByIdAsync(long commentId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Lesson)
                    .Include(c => c.InverseParentComment)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return ApiResponse<CommentDto>.ErrorResponse(CommentConstants.Messages.CommentNotFound);
                }

                var commentDto = MapToDto(comment);
                return ApiResponse<CommentDto>.SuccessResponse(commentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting comment by id: {CommentId}", commentId);
                return ApiResponse<CommentDto>.ErrorResponse(Messages.ErrorRetrievingComment);
            }
        }

        public async Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentRequest request, string userId)
        {
            try
            {
                // Validate lesson exists
                var lesson = await _context.Lessons.FindAsync(request.LessonId);
                if (lesson == null)
                {
                    return ApiResponse<CommentDto>.ErrorResponse(Messages.LessonNotFound);
                }

                // Validate parent comment if provided
                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _context.Comments.FindAsync(request.ParentCommentId.Value);
                    if (parentComment == null)
                    {
                        return ApiResponse<CommentDto>.ErrorResponse(Messages.ParentCommentNotFound);
                    }
                }

                // Content moderation
                if (ContainsInappropriateContent(request.Content))
                {
                    return ApiResponse<CommentDto>.ErrorResponse(CommentConstants.Messages.InvalidCommentContent);
                }

                var comment = new Comment
                {
                    Content = request.Content,
                    UserId = userId,
                    LessonId = request.LessonId,
                    ParentCommentId = request.ParentCommentId,
                    CreatedAt = DateTime.UtcNow,
                    Status = CommentConstants.Status.Pending // Default to pending for moderation
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Reload with includes
                comment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Lesson)
                    .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);

                var commentDto = MapToDto(comment!);
                return ApiResponse<CommentDto>.SuccessResponse(commentDto, CommentConstants.Messages.CommentCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating comment");
                return ApiResponse<CommentDto>.ErrorResponse(Messages.ErrorCreatingComment);
            }
        }

        public async Task<ApiResponse<CommentDto>> UpdateCommentAsync(long commentId, UpdateCommentRequest request, string userId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Lesson)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return ApiResponse<CommentDto>.ErrorResponse(CommentConstants.Messages.CommentNotFound);
                }

                // Check if user owns the comment
                if (comment.UserId != userId)
                {
                    return ApiResponse<CommentDto>.ErrorResponse(CommentConstants.Messages.UnauthorizedAccess);
                }

                // Content moderation
                if (ContainsInappropriateContent(request.Content))
                {
                    return ApiResponse<CommentDto>.ErrorResponse(CommentConstants.Messages.InvalidCommentContent);
                }

                comment.Content = request.Content;
                comment.UpdatedAt = DateTime.UtcNow;
                comment.Status = CommentConstants.Status.Pending; // Reset to pending after edit

                await _context.SaveChangesAsync();

                var commentDto = MapToDto(comment);
                return ApiResponse<CommentDto>.SuccessResponse(commentDto, CommentConstants.Messages.CommentUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating comment: {CommentId}", commentId);
                return ApiResponse<CommentDto>.ErrorResponse(Messages.ErrorUpdatingComment);
            }
        }

        public async Task<ApiResponse<bool>> DeleteCommentAsync(long commentId, string userId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.InverseParentComment)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(CommentConstants.Messages.CommentNotFound);
                }

                // Check if user owns the comment
                if (comment.UserId != userId)
                {
                    return ApiResponse<bool>.ErrorResponse(CommentConstants.Messages.UnauthorizedAccess);
                }

                // Delete all replies first
                if (comment.InverseParentComment.Any())
                {
                    _context.Comments.RemoveRange(comment.InverseParentComment);
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, CommentConstants.Messages.CommentDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting comment: {CommentId}", commentId);
                return ApiResponse<bool>.ErrorResponse(Messages.ErrorDeletingComment);
            }
        }

        public async Task<ApiResponse<bool>> ModerateCommentAsync(CommentModerationRequest request, string adminId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(request.CommentId);
                if (comment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(CommentConstants.Messages.CommentNotFound);
                }

                comment.Status = request.Action;
                comment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var message = request.Action switch
                {
                    CommentConstants.Status.Approved => CommentConstants.Messages.CommentApproved,
                    CommentConstants.Status.Rejected => CommentConstants.Messages.CommentRejected,
                    _ => Messages.CommentStatusUpdated
                };

                _logger.LogInformation("Comment {CommentId} moderated by admin {AdminId} with action {Action}",
                    request.CommentId, adminId, request.Action);

                return ApiResponse<bool>.SuccessResponse(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moderating comment: {CommentId}", request.CommentId);
                return ApiResponse<bool>.ErrorResponse(Messages.ErrorModeratingComment);
            }
        }

        public async Task<ApiResponse<List<CommentDto>>> GetCommentsByLessonAsync(long lessonId)
        {
            try
            {
                var comments = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.InverseParentComment)
                        .ThenInclude(c => c.User)
                    .Where(c => c.LessonId == lessonId && c.Status == CommentConstants.Status.Approved)
                    .Where(c => c.ParentCommentId == null) // Only get parent comments
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var commentDtos = comments.Select(c => MapToDto(c, true)).ToList();
                return ApiResponse<List<CommentDto>>.SuccessResponse(commentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting comments for lesson: {LessonId}", lessonId);
                return ApiResponse<List<CommentDto>>.ErrorResponse(Messages.ErrorRetrievingLessonComments);
            }
        }

        public async Task<ApiResponse<PagedResult<CommentDto>>> GetPendingCommentsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Lesson)
                    .Where(c => c.Status == CommentConstants.Status.Pending)
                    .OrderByDescending(c => c.CreatedAt);

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var comments = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var commentDtos = comments.Select(c => MapToDto(c, true)).ToList();

                var result = new PagedResult<CommentDto>
                {
                    Data = commentDtos,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    HasNext = page < totalPages,
                    HasPrevious = page > 1
                };

                return ApiResponse<PagedResult<CommentDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending comments");
                return ApiResponse<PagedResult<CommentDto>>.ErrorResponse(Messages.ErrorRetrievingPendingComments);
            }
        }

        private static CommentDto MapToDto(Comment comment, bool includeReplies = false)
        {
            return new CommentDto
            {
                CommentId = comment.CommentId,
                ParentCommentId = comment.ParentCommentId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                UserId = comment.UserId,
                LessonId = comment.LessonId,
                UserName = comment.User.FullName,
                UserEmail = comment.User.Email,
                UserAvatarUrl = comment.User.AvatarUrl,
                LessonName = comment.Lesson?.LessonName,
                ModuleName = comment.Lesson?.Module?.ModuleName,
                CourseName = comment.Lesson?.Module?.Course?.CourseName,
                Status = comment.Status,
                StatusText = GetStatusText(comment.Status),
                ReplyCount = comment.InverseParentComment?.Count ?? 0,
                IsReported = false,
                ReportCount = 0,
                Replies = includeReplies && comment.InverseParentComment != null
                    ? comment.InverseParentComment
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => MapToDto(c, false)) // CHỈ map 1 cấp reply
                        .ToList()
                    : new List<CommentDto>()
            };
        }


        private static string GetStatusText(int? status)
        {
            return status switch
            {
                CommentConstants.Status.Pending => "Pending",
                CommentConstants.Status.Approved => "Approved",
                CommentConstants.Status.Rejected => "Rejected",
                _ => "Unknown"
            };
        }



        private static bool ContainsInappropriateContent(string content)
        {
            // Simple content moderation - can be enhanced with AI services
            var inappropriateWords = new[]
            {
                "spam", "scam", "fake", "cheat", "hack", "illegal",
                // Add more inappropriate words as needed
            };

            return inappropriateWords.Any(word =>
                content.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ApiResponse<bool>> ReportCommentAsync(CommentReportRequest request, string userId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(request.CommentId);
                if (comment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(CommentConstants.Messages.CommentNotFound);
                }

                // Không lưu trạng thái, chỉ ghi log
                _logger.LogInformation("Comment {CommentId} reported by user {UserId} with reason '{Reason}' and type {Type}",
                    request.CommentId, userId, request.Reason, request.ReportType);

                return ApiResponse<bool>.SuccessResponse(true, CommentConstants.Messages.CommentReported);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reporting comment: {CommentId}", request.CommentId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while reporting the comment");
            }
        }


        public async Task<ApiResponse<bool>> BulkModerateCommentsAsync(BulkCommentModerationRequest request, string adminId)
        {
            try
            {
                var comments = await _context.Comments
                    .Where(c => request.CommentIds.Contains(c.CommentId))
                    .ToListAsync();

                if (!comments.Any())
                {
                    return ApiResponse<bool>.ErrorResponse(Messages.NoValidCommentsForModeration);
                }

                foreach (var comment in comments)
                {
                    comment.Status = request.Action;
                    comment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin {AdminId} bulk moderated {Count} comments to status {Status}",
                    adminId, comments.Count, request.Action);

                return ApiResponse<bool>.SuccessResponse(true, Messages.BulkModerationSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during bulk moderation");
                return ApiResponse<bool>.ErrorResponse(Messages.ErrorBulkModeration);
            }
        }



        public async Task<ApiResponse<CommentStatisticsDto>> GetCommentStatisticsAsync()
        {
            try
            {
                var totalComments = await _context.Comments.CountAsync();
                var pending = await _context.Comments.CountAsync(c => c.Status == CommentConstants.Status.Pending);
                var approved = await _context.Comments.CountAsync(c => c.Status == CommentConstants.Status.Approved);
                var rejected = await _context.Comments.CountAsync(c => c.Status == CommentConstants.Status.Rejected);

                var mostActiveUser = await _context.Comments
                    .GroupBy(c => c.User.FullName)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync() ?? "N/A";

                var dto = new CommentStatisticsDto
                {
                    TotalComments = totalComments,
                    PendingCount = pending,
                    ApprovedCount = approved,
                    RejectedCount = rejected,
                    ReportedCount = 0, // không có trường nên fix cứng
                    MostActiveUser = mostActiveUser
                };

                return ApiResponse<CommentStatisticsDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving comment statistics");
                return ApiResponse<CommentStatisticsDto>.ErrorResponse(Messages.ErrorRetrievingCommentStatistics);
            }
        }
    }
}
