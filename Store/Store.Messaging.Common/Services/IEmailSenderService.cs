using System;
using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface IEmailSenderService
    {
        Task SendConfirmAccountEmailAsync(Guid clientId, string email, string url);

        Task SendConfirmExternalAccountEmailAsync(Guid clientId, string email, string url, string provider);

        Task SendResetPasswordEmailAsync(Guid clientId, string email, string url, string userName);

        Task SendChangePasswordEmailAsync(Guid clientId, string email, string userName, string newPassword);
    }
}
