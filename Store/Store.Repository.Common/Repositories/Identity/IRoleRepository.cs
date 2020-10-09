using System;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleRepository : IDapperGenericRepository<IRole, Guid>
    {
        IRole FindByName(string roleName);
    }
}