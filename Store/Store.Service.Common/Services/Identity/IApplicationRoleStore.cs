using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Service.Common.Services.Identity
{
    public interface IApplicationRoleStore<TRole> : IRoleStore<TRole>, IDisposable where TRole : class, IRole
    {
        Task<IEnumerable<TRole>> GetRolesAsync();
    }
}