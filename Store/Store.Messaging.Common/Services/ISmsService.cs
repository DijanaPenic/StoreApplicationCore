using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface ISmsService
    {
        Task SendSmsAsync(string phoneNumber, string body);
    }
}