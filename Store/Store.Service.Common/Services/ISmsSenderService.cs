using System.Threading.Tasks;

namespace Store.Service.Common.Services
{
    public interface ISmsSenderService
    {
        Task SendSmsAsync(string phoneNumber, string body);
    }
}
