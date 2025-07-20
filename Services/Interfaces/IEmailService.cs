namespace Online_Learning.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendMail(string toEmail, string subject, string body);
    }
}
