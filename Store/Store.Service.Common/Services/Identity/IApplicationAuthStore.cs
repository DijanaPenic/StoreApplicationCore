using System;

namespace Store.Service.Common.Services.Identity
{
    public interface IApplicationAuthStore : IClientStore, IUserRefreshTokenStore, IDisposable
    {

    }
}