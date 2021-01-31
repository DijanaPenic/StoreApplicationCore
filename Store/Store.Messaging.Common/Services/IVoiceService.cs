using System;
using System.Threading.Tasks;

namespace Store.Messaging.Services.Common
{
    public interface IVoiceService
    {
        Task CallAsync(string phoneNumber, Uri url);
    }
}