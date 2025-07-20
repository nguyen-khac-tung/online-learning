using Online_Learning.Models.DTOs.Rating;

namespace Online_Learning.Services.Interfaces
{
    public interface IRatingService
    {
        Task<RatingResponse> CreateAsync(string userId, CreateRatingRequest req);
        Task<IEnumerable<RatingResponse>> GetByCourseAsync(string courseId);
        Task<double> GetAverageAsync(string courseId);
        Task<RatingResponse?> GetByUserCourseAsync(string userId, string courseId);
        Task<bool> DeleteAsync(long ratingId);
        Task<(IEnumerable<RatingResponse>, int)> GetByCoursePagedAsync(string courseId, int page, int pageSize);
    }
}
