using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly OnlineLearningContext _context;
        public UserOtpRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public UserOtp GetOtpByEmail(string email)
        {
            return _context.UserOtps.FirstOrDefault(o => o.Email == email);
        }
        public void CreateOtp(UserOtp userOtp)
        {
            _context.UserOtps.Add(userOtp);
            _context.SaveChanges();
        }

        public void UpdateOtp(UserOtp userOtp)
        {
            _context.UserOtps.Update(userOtp);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
