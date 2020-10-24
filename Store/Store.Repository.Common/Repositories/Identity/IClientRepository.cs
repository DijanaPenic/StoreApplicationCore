using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IClientRepository
    {
        Task<IEnumerable<IClient>> GetAsync();

        Task<IClient> FindByKeyAsync(Guid key);

        Task<IClient> FindByNameAsync(string name);
    }
}