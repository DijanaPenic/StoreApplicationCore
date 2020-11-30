using System.Threading.Tasks;

namespace Store.Service.Common.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
