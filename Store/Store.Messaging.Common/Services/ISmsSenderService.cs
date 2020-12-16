using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface ISmsSenderService
    {
        Task SendSmsAsync(string phoneNumber, string body);
    }
}
