using System.Threading.Tasks;
using Application.Interfaces;
using Utilities.Helper;

namespace Application.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfiguration _configuration;

        public EmailService(SmtpConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public Task SendConfirmPasswordEmail(string to, string password)
        {
            var task = new Task(() => EmailHelpers.SendEmail(to
                , "Warehouse System Manager - Your Password"
                , "This is your password: " + password
                , _configuration.UserName
                , _configuration.Password));
            task.Start();
            return task;
        }
    }
    
}