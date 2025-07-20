using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating> AddAsync(Rating rating);
        Task<IEnumerable<Rating>> GetByCourseAsync(string courseId);
        Task<double> GetAverageScoreAsync(string courseId);
        Task<Rating?> GetByUserCourseAsync(string userId, string courseId);
        Task<Rating?> GetByIdAsync(long ratingId);
        Task DeleteAsync(Rating rating);
        Task<(IEnumerable<Rating>, int)> GetPagedByCourseAsync(string courseId, int page, int pageSize);
    }
}
