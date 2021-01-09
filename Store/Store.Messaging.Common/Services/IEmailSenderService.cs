using System;
using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface IEmailSenderService
    {
        Task SendConfirmAccountAsync(Guid clientId, string email, string url);

        Task SendConfirmEmailAsync(Guid clientId, string email, string url, string userName);

        Task SendConfirmExternalAccountAsync(Guid clientId, string email, string url, string provider);

        Task SendResetPasswordAsync(Guid clientId, string email, string url, string userName);

        Task SendChangePasswordEmailAsync(Guid clientId, string email, string userName, string newPassword);
    }
}
