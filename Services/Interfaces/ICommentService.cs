using Online_Learning.Models.DTOs.Comment;
using Online_Learning.Models.DTOs.Common;

namespace Online_Learning.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<PagedResult<CommentDto>>> GetCommentsAsync(CommentFilterRequest request);
        Task<ApiResponse<CommentDto>> GetCommentByIdAsync(long commentId);
        Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentRequest request, string userId);
        Task<ApiResponse<CommentDto>> UpdateCommentAsync(long commentId, UpdateCommentRequest request, string userId);
        Task<ApiResponse<bool>> DeleteCommentAsync(long commentId, string userId);
        Task<ApiResponse<bool>> ModerateCommentAsync(CommentModerationRequest request, string adminId);
        Task<ApiResponse<List<CommentDto>>> GetCommentsByLessonAsync(long lessonId);
        Task<ApiResponse<PagedResult<CommentDto>>> GetPendingCommentsAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse<bool>> ReportCommentAsync(CommentReportRequest request, string userId);
        Task<ApiResponse<bool>> BulkModerateCommentsAsync(BulkCommentModerationRequest request, string adminId);
        Task<ApiResponse<CommentStatisticsDto>> GetCommentStatisticsAsync();
    }
}
