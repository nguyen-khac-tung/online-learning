using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Implementations
{
    public class RatingRepository : IRatingRepository
    {
        private readonly OnlineLearningContext _ctx;
        public RatingRepository(OnlineLearningContext ctx) => _ctx = ctx;

        public async Task<Rating> AddAsync(Rating rating)
        {
            _ctx.Ratings.Update(rating);
            await _ctx.SaveChangesAsync();
            return rating;
        }

        public async Task<IEnumerable<Rating>> GetByCourseAsync(string courseId) =>
            await _ctx.Ratings.Where(r => r.CourseId == courseId).ToListAsync();

        public async Task<double> GetAverageScoreAsync(string courseId) =>
            await _ctx.Ratings.Where(r => r.CourseId == courseId).AverageAsync(r => (double)r.Score);

        public async Task<Rating?> GetByUserCourseAsync(string userId, string courseId) =>
            await _ctx.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

        public async Task<Rating?> GetByIdAsync(long ratingId) =>
            await _ctx.Ratings.FindAsync(ratingId);

        public async Task DeleteAsync(Rating rating)
        {
            _ctx.Ratings.Remove(rating);
            await _ctx.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Rating>, int)> GetPagedByCourseAsync(string courseId, int page, int pageSize)
        {
            var query = _ctx.Ratings.Where(r => r.CourseId == courseId);
            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, total);
        }
    }
}
