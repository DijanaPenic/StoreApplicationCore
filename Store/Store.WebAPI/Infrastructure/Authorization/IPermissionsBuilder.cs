using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Infrastructure.Authorization
{
    public interface IPermissionsBuilder
    {
        Task<IList<Claim>> BuildRoleClaims(IUser user);
    }
}