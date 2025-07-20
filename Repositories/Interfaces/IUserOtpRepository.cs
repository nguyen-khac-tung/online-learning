using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IUserOtpRepository
    {
        UserOtp GetOtpByEmail(string email);
        void CreateOtp(UserOtp userOtp);
        void UpdateOtp(UserOtp userOtp);
        int SaveChanges();
    }
}
