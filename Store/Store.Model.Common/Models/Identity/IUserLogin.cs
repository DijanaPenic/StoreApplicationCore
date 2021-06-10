using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserLogin : IUserLoginKey, IBaseEntity, IChangable
    {
        string ProviderDisplayName { get; set; }

        string Token { get; set; }

        bool IsConfirmed { get; set; }

        Guid UserId { get; set; }
    }

    public interface IUserLoginKey
    {
        string LoginProvider { get; set; }

        string ProviderKey { get; set; }
    }
}