using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Service.Common.Services.Identity
{
    public interface IClientStore
    {
        Task<IClient> FindClientByIdAsync(Guid clientId);

        Task<IClient> FindClientByNameAsync(string name);
    }
}