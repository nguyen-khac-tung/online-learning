using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.Course;
using Online_Learning.Repositories.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Online_Learning.Models.DTOs.Request.Admin.Course;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;
using Online_Learning.Constants;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class CourseRepository : ICourseRepository
    {
        private readonly OnlineLearningContext _context;
        public CourseRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesAsync(int page, int pageSize, string? search, int? status)
        {
            var query = _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.CourseImages)
                .Include(c => c.CoursePrices)
                .Include(c => c.CourseCategories)
                .Where(c => c.DeletedAt == null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search) || (c.Description != null && c.Description.Contains(search)));
            }
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseResponseDto
                {
                    CourseID = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Acceptor = c.Acceptor,
                    Creator = c.Creator,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    PublishedAt = c.PublishedAt,
                    StudyTime = c.StudyTime,
                    LevelID = c.LevelId,
                    LanguageID = c.LanguageId,
                    ImageUrls = c.CourseImages.Select(img => img.ImageUrl).ToList(),
                    ModuleCount = c.Modules.Count,
                    CurrentPrice = c.CoursePrices.Any() ? c.CoursePrices.OrderByDescending(cp => cp.CreateAt).FirstOrDefault()!.Price : null,
                    PriceHistory = c.CoursePrices.OrderByDescending(cp => cp.CreateAt).Take(5).Select(cp => new CoursePriceResponseDto
                    {
                        CoursePriceID = cp.CoursePriceId,
                        CourseID = c.CourseId,
                        CourseName = c.CourseName,
                        Price = cp.Price,
                        CreateAt = cp.CreateAt
                    }).ToList(),
                    CategoryIDs = c.CourseCategories.Select(cc => cc.CategoryId).ToList()
                })
                .ToListAsync();

            // Đảm bảo CurrentPrice không null nếu có PriceHistory
            foreach (var course in courses)
            {
                if (course.CurrentPrice == null && course.PriceHistory.Any())
                {
                    course.CurrentPrice = course.PriceHistory.OrderByDescending(ph => ph.CreateAt).First().Price;
                }
            }
            return courses;
        }

        public async Task<CourseResponseDto?> GetCourseByIdAsync(string id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.CourseImages)
                .Include(c => c.CoursePrices)
                .Include(c => c.CourseCategories)
                .Where(c => c.CourseId == id && c.DeletedAt == null)
                .Select(c => new CourseResponseDto
                {
                    CourseID = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Acceptor = c.Acceptor,
                    Creator = c.Creator,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    PublishedAt = c.PublishedAt,
                    StudyTime = c.StudyTime,
                    LevelID = c.LevelId,
                    LanguageID = c.LanguageId,
                    ImageUrls = c.CourseImages.Select(img => img.ImageUrl).ToList(),
                    ModuleCount = c.Modules.Count,
                    CurrentPrice = c.CoursePrices.Any() ? c.CoursePrices.OrderByDescending(cp => cp.CreateAt).FirstOrDefault()!.Price : null,
                    PriceHistory = c.CoursePrices.OrderByDescending(cp => cp.CreateAt).Take(5).Select(cp => new CoursePriceResponseDto
                    {
                        CoursePriceID = cp.CoursePriceId,
                        CourseID = c.CourseId,
                        CourseName = c.CourseName,
                        Price = cp.Price,
                        CreateAt = cp.CreateAt
                    }).ToList(),
                    CategoryIDs = c.CourseCategories.Select(cc => cc.CategoryId).ToList()
                })
                .FirstOrDefaultAsync();

            if (course != null && course.CurrentPrice == null && course.PriceHistory.Any())
            {
                course.CurrentPrice = course.PriceHistory.OrderByDescending(ph => ph.CreateAt).First().Price;
            }
            return course;
        }

        public async Task<int> GetTotalCountAsync(string? search, int? status)
        {
            var query = _context.Courses.Where(c => c.DeletedAt == null);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search) || (c.Description != null && c.Description.Contains(search)));
            }
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }
            return await query.CountAsync();
        }

        // Thêm các method thao tác dữ liệu mà trước đây nằm ở Service
        public async Task<(Course, List<string>, CoursePrice?)> CreateCourseAsync(Course course, List<int>? categoryIds, decimal? price, List<IFormFile>? attachmentFiles, IFileService fileService)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                if (categoryIds?.Any() == true)
                {
                    foreach (var categoryId in categoryIds)
                    {
                        if (await _context.Categories.AnyAsync(c => c.CategoryId == categoryId && c.DeletedAt == null))
                        {
                            _context.CourseCategories.Add(new CourseCategory { CourseId = course.CourseId, CategoryId = categoryId });
                        }
                    }
                }

                CoursePrice? coursePrice = null;
                if (price.HasValue)
                {
                    coursePrice = new CoursePrice
                    {
                        CourseId = course.CourseId,
                        Price = price.Value,
                        CreateAt = DateTime.UtcNow
                    };
                    _context.CoursePrices.Add(coursePrice);
                }

                List<string> uploadedFiles = new();
                if (attachmentFiles?.Any() == true)
                {
                    uploadedFiles = await fileService.UploadFilesAsync(attachmentFiles, "courses");
                    if (uploadedFiles.Any())
                    {
                        _context.CourseImages.Add(new CourseImage { CourseId = course.CourseId, ImageUrl = uploadedFiles.First() });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (course, uploadedFiles, coursePrice);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateCourseAsync(string id, CourseUpdateDto courseDto, IFileService fileService)
        {
            var course = await _context.Courses
                .Include(c => c.CourseImages)
                .Include(c => c.CoursePrices)
                .Include(c => c.CourseCategories)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null || course.DeletedAt != null || course.Status == -1)
            {
                return false;
            }
            if ((courseDto.AttachmentFiles?.Count ?? 0) > 1)
            {
                throw new InvalidOperationException(Messages.CannotUploadMoreThanOneImage);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                course.CourseName = courseDto.CourseName;
                course.Description = courseDto.Description;
                course.StudyTime = courseDto.StudyTime;
                course.LevelId = courseDto.LevelID;
                course.LanguageId = courseDto.LanguageID;
                course.Status = courseDto.Status;
                course.UpdatedAt = DateTime.UtcNow;

                if (courseDto.CategoryIDs != null)
                {
                    _context.CourseCategories.RemoveRange(course.CourseCategories);
                    foreach (var categoryId in courseDto.CategoryIDs)
                    {
                        if (await _context.Categories.AnyAsync(c => c.CategoryId == categoryId && c.DeletedAt == null))
                        {
                            _context.CourseCategories.Add(new CourseCategory { CourseId = course.CourseId, CategoryId = categoryId });
                        }
                    }
                }

                var maxCoursePriceId = await _context.CoursePrices.MaxAsync(cp => (long?)cp.CoursePriceId);
                int nextCoursePriceId = maxCoursePriceId.HasValue ? (int)(maxCoursePriceId.Value + 1) : 1;
                if (courseDto.Price.HasValue)
                {
                    var currentPrice = course.CoursePrices.OrderByDescending(cp => cp.CreateAt).FirstOrDefault();
                    if (currentPrice == null || currentPrice.Price != courseDto.Price.Value)
                    {
                        _context.CoursePrices.Add(new CoursePrice
                        {
                            CourseId = course.CourseId,
                            Price = courseDto.Price.Value,
                            CreateAt = DateTime.UtcNow
                        });
                    }
                }

                var existingImage = course.CourseImages.FirstOrDefault();
                bool shouldRemoveExisting = false;
                if (existingImage != null && courseDto.RemovedImageUrls?.Contains(existingImage.ImageUrl) == true)
                {
                    shouldRemoveExisting = true;
                }
                else if (existingImage != null && (courseDto.AttachmentFiles?.Any() == true))
                {
                    shouldRemoveExisting = true;
                }
                if (shouldRemoveExisting)
                {
                    _context.CourseImages.Remove(existingImage!);
                    await fileService.DeleteFileAsync(existingImage!.ImageUrl);
                }
                if (courseDto.AttachmentFiles?.Any() == true)
                {
                    var uploadedFiles = await fileService.UploadFilesAsync(courseDto.AttachmentFiles, "courses");
                    if (uploadedFiles.Any())
                    {
                        _context.CourseImages.Add(new CourseImage { CourseId = course.CourseId, ImageUrl = uploadedFiles.First() });
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCourseAsync(string id, IFileService fileService)
        {
            var course = await _context.Courses.Include(c => c.CourseImages).FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
            {
                return false;
            }
            course.DeletedAt = DateTime.UtcNow;
            course.Status = -1;
            if (course.CourseImages.Any())
            {
                var filePaths = course.CourseImages.Select(img => img.ImageUrl).ToList();
                await fileService.DeleteFilesAsync(filePaths);
                _context.CourseImages.RemoveRange(course.CourseImages);
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}