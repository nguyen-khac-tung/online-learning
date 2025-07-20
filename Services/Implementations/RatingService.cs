using Online_Learning.Models.DTOs.Rating;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _repo;
        private readonly ICourseEnrollmentRepository _enrollRepo;

        public RatingService(IRatingRepository repo, ICourseEnrollmentRepository enrollRepo)
        {
            _repo = repo;
            _enrollRepo = enrollRepo;
        }

        public async Task<RatingResponse> CreateAsync(string userId, CreateRatingRequest req)
        {
            var hasCompleted = await _enrollRepo.HasCompletedCourseAsync(userId, req.CourseID);
            if (!hasCompleted)
                throw new InvalidOperationException("Bạn chỉ có thể đánh giá sau khi hoàn thành khóa học.");

            var existing = await _repo.GetByUserCourseAsync(userId, req.CourseID);
            if (existing != null)
            {
                existing.Score = req.Score;
                existing.ReviewText = req.ReviewText;
                existing.UpdatedAt = DateTime.Now;
                await _repo.AddAsync(existing);
                return Map(existing);
            }

            var rating = new Rating
            {
                CourseId = req.CourseID,
                UserId = userId,
                Score = req.Score,
                ReviewText = req.ReviewText,
                CreatedAt = DateTime.Now
            };
            var created = await _repo.AddAsync(rating);
            return Map(created);
        }

        public async Task<IEnumerable<RatingResponse>> GetByCourseAsync(string courseId) =>
            (await _repo.GetByCourseAsync(courseId)).Select(Map);

        public Task<double> GetAverageAsync(string courseId) =>
            _repo.GetAverageScoreAsync(courseId);

        public async Task<RatingResponse?> GetByUserCourseAsync(string userId, string courseId)
        {
            var rating = await _repo.GetByUserCourseAsync(userId, courseId);
            return rating == null ? null : Map(rating);
        }

        public async Task<bool> DeleteAsync(long ratingId)
        {
            var rating = await _repo.GetByIdAsync(ratingId);
            if (rating == null) return false;
            await _repo.DeleteAsync(rating);
            return true;
        }

        public async Task<(IEnumerable<RatingResponse>, int)> GetByCoursePagedAsync(string courseId, int page, int pageSize)
        {
            var (data, total) = await _repo.GetPagedByCourseAsync(courseId, page, pageSize);
            return (data.Select(Map), total);
        }

        private static RatingResponse Map(Rating r) => new RatingResponse
        {
            RatingID = r.RatingId,
            CourseID = r.CourseId,
            UserID = r.UserId,
            Score = r.Score,
            ReviewText = r.ReviewText,
            CreatedAt = r.CreatedAt
        };
    }
}
