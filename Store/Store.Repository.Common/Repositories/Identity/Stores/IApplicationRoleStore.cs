using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleStore<TRole> : IRoleStore<TRole>, IDisposable where TRole : class, IRole
    {
        Task<IEnumerable<TRole>> GetRolesAsync();
    }
}