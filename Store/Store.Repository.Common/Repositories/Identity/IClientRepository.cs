using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IClientRepository
    {
        Task<IEnumerable<IClient>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null);

        Task<IClient> FindByKeyAsync(Guid key, IOptionsParameters options = null);

        Task<IClient> FindByNameAsync(string name);
    }
}