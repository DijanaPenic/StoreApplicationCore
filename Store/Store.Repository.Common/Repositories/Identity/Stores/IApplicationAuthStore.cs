using System;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationAuthStore : IClientStore, IUserRefreshTokenStore, IDisposable
    {

    }
}