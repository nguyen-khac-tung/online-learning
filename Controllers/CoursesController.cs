using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.Admin.Course;
using Online_Learning.Models.DTOs.Response.Admin.Course;
using Online_Learning.Models.Entities;
using Online_Learning.Services;
using Online_Learning.Services.Interfaces;


namespace Online_Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly OnlineLearningContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(OnlineLearningContext context, IFileService fileService, ILogger<CoursesController> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? status = null)
        {
            var query = _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.CourseImages)
                .Include(c => c.CoursePrices)
                .Include(c => c.CourseCategories)
                .Where(c => c.DeletedAt == null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search) ||
                                       (c.Description != null && c.Description.Contains(search)));
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

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

            foreach (var course in courses)
            {
                if (course.CurrentPrice == null && course.PriceHistory.Any())
                {
                    course.CurrentPrice = course.PriceHistory.OrderByDescending(ph => ph.CreateAt).First().Price;
                }
            }

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(courses);
        }

        // GET: api/Courses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetCourse(string id)
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

            if (course == null)
            {
                return NotFound();
            }

            if (course.CurrentPrice == null && course.PriceHistory.Any())
            {
                course.CurrentPrice = course.PriceHistory.OrderByDescending(ph => ph.CreateAt).First().Price;
            }

            return Ok(course);
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse([FromForm] CourseCreateDto courseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate image count: Allow only 1 image
            if (courseDto.AttachmentFiles?.Count > 1)
            {
                return BadRequest("Cannot upload more than 1 image.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var course = new Course
                {
                    CourseId = Guid.NewGuid().ToString(), // Ensure unique CourseId
                    CourseName = courseDto.CourseName,
                    Description = courseDto.Description,
                    Creator = courseDto.Creator,
                    StudyTime = courseDto.StudyTime,
                    LevelId = courseDto.LevelID,
                    LanguageId = courseDto.LanguageID,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                if (courseDto.CategoryIDs?.Any() == true)
                {
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

                CoursePrice? coursePrice = null;
                if (courseDto.Price.HasValue)
                {
                    coursePrice = new CoursePrice
                    {
                        CoursePriceId = nextCoursePriceId,
                        CourseId = course.CourseId,
                        Price = courseDto.Price.Value,
                        CreateAt = DateTime.UtcNow
                    };
                    _context.CoursePrices.Add(coursePrice);
                }

                List<string> uploadedFiles = new();
                if (courseDto.AttachmentFiles?.Any() == true) // Only one file expected due to validation above
                {
                    try
                    {
                        // UploadFilesAsync expects a List<IFormFile>, so pass the single file in a list
                        uploadedFiles = await _fileService.UploadFilesAsync(courseDto.AttachmentFiles, "courses");

                        // Assuming uploadedFiles will contain at most one URL
                        if (uploadedFiles.Any())
                        {
                            var maxCourseImageId = await _context.CourseImages.MaxAsync(ci => (long?)ci.ImageID);
                            int nextCourseImageId = maxCourseImageId.HasValue ? (int)(maxCourseImageId.Value + 1) : 1;
                            _context.CourseImages.Add(new CourseImage { ImageID = nextCourseImageId, CourseID = course.CourseId, ImageUrl = uploadedFiles.First() });
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Invalid file upload for course {CourseName}", courseDto.CourseName);
                        await transaction.RollbackAsync();
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading files for course {CourseName}", courseDto.CourseName);
                        await transaction.RollbackAsync();
                        return StatusCode(500, "Error uploading files");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new CourseResponseDto
                {
                    CourseID = course.CourseId,
                    CourseName = course.CourseName,
                    Description = course.Description,
                    Creator = course.Creator,
                    Status = course.Status,
                    CreatedAt = course.CreatedAt,
                    StudyTime = course.StudyTime,
                    LevelID = course.LevelId,
                    LanguageID = course.LanguageId,
                    ImageUrls = uploadedFiles, // This will contain the single uploaded URL or be empty
                    ModuleCount = 0,
                    CurrentPrice = courseDto.Price,
                    PriceHistory = coursePrice != null ? new List<CoursePriceResponseDto>
                    {
                        new CoursePriceResponseDto
                        {
                            CoursePriceID = coursePrice.CoursePriceId,
                            CourseID = course.CourseId,
                            CourseName = course.CourseName,
                            Price = coursePrice.Price,
                            CreateAt = coursePrice.CreateAt
                        }
                    } : new List<CoursePriceResponseDto>(),
                    CategoryIDs = courseDto.CategoryIDs ?? new List<int>()
                };

                return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, response);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating course {CourseName}", courseDto.CourseName);
                await transaction.RollbackAsync();
                return StatusCode(500, "Database error: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create course {CourseName}", courseDto.CourseName);
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Courses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromForm] CourseUpdateDto courseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
                .Include(c => c.CourseImages)
                .Include(c => c.CoursePrices)
                .Include(c => c.CourseCategories)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                _logger.LogWarning("Course {CourseId} not found", id);
                return NotFound();
            }

            if (course.DeletedAt != null || course.Status == -1)
            {
                _logger.LogWarning("Attempt to update deleted course {CourseId}", id);
                return BadRequest("Cannot update a deleted course");
            }

            // Validate new image count: Allow at most 1 new image
            if ((courseDto.AttachmentFiles?.Count ?? 0) > 1)
            {
                return BadRequest("Cannot upload more than 1 new image.");
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
                            CoursePriceId = nextCoursePriceId,
                            CourseId = course.CourseId,
                            Price = courseDto.Price.Value,
                            CreateAt = DateTime.UtcNow
                        });
                    }
                }

                // --- Image handling logic for single image ---
                var existingImage = course.CourseImages.FirstOrDefault(); // Get the current single image

                bool shouldRemoveExisting = false;
                // Case 1: User explicitly wants to remove the existing image
                if (existingImage != null && courseDto.RemovedImageUrls?.Contains(existingImage.ImageUrl) == true)
                {
                    shouldRemoveExisting = true;
                }
                // Case 2: User is uploading a new image, so the old one must be replaced
                else if (existingImage != null && (courseDto.AttachmentFiles?.Any() == true))
                {
                    shouldRemoveExisting = true;
                }

                if (shouldRemoveExisting)
                {
                    _context.CourseImages.Remove(existingImage!);
                    var deleteResult = await _fileService.DeleteFileAsync(existingImage!.ImageUrl);
                    _logger.LogInformation("Removed existing image {ImageUrl} for course {CourseId}, DeleteResult: {Result}",
                        existingImage!.ImageUrl, id, deleteResult);
                }

                // Handle adding new image
                if (courseDto.AttachmentFiles?.Any() == true) // Expecting only one file due to validation
                {
                    try
                    {
                        var uploadedFiles = await _fileService.UploadFilesAsync(courseDto.AttachmentFiles, "courses");
                        if (uploadedFiles.Any())
                        {
                            var maxCourseImageId = await _context.CourseImages.MaxAsync(ci => (long?)ci.ImageID);
                            int nextCourseImageId = maxCourseImageId.HasValue ? (int)(maxCourseImageId.Value + 1) : 1;
                            _context.CourseImages.Add(new CourseImage { ImageID = nextCourseImageId, CourseID = course.CourseId, ImageUrl = uploadedFiles.First() });
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Invalid file upload for course {CourseId}", id);
                        await transaction.RollbackAsync();
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading files for course {CourseId}", id);
                        await transaction.RollbackAsync();
                        return StatusCode(500, "Error uploading files");
                    }
                }
                // --- End image handling logic ---

                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Saved {Changes} changes for course {CourseId}", changes, id);
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating course {CourseId}", id);
                await transaction.RollbackAsync();
                return StatusCode(500, "Database error: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update course {CourseId}", id);
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Courses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseImages)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            course.DeletedAt = DateTime.UtcNow;
            course.Status = -1;

            if (course.CourseImages.Any())
            {
                var filePaths = course.CourseImages.Select(img => img.ImageUrl).ToList();
                await _fileService.DeleteFilesAsync(filePaths);
                _context.CourseImages.RemoveRange(course.CourseImages);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}