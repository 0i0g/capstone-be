using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmPasswordEmail(string to, string password);
    }
}