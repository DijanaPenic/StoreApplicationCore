using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IClientStore
    {
        Task<IClient> FindClientByKeyAsync(Guid clientId);

        Task<IClient> FindClientByNameAsync(string name);
    }
}