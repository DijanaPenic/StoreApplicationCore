using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface IEmailSenderService
    {
        Task SendConfirmAccountEmailAsync(string email, string url);

        Task SendConfirmExternalAccountEmailAsync(string email, string url, string provider);

        Task SendResetPasswordEmailAsync(string email, string url, string userName);

        Task SendChangePasswordEmailAsync(string email, string userName, string newPassword);
    }
}
