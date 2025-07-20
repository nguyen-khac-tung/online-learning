using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces
{
    public interface ICourseEnrollmentRepository
    {
        Task<bool> HasCompletedCourseAsync(string userId, string courseId);
    }
}
