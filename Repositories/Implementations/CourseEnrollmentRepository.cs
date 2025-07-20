using Microsoft.EntityFrameworkCore;
using Online_Learning.Models;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class CourseEnrollmentRepository : ICourseEnrollmentRepository
    {
        private readonly OnlineLearningContext _context;

        public CourseEnrollmentRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<bool> HasCompletedCourseAsync(string userId, string courseId)
        {
            return await _context.CourseEnrollments
                .AnyAsync(e => e.UserId == userId
                            && e.CourseId == courseId
                            && (e.Status == 1 || e.Progress >= 100));
        }
    }
}
