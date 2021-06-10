using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserRefreshToken : IUserRefreshTokenKey, IBaseEntity, IChangable
    {
        string Value { get; set; }

        IUser User { get; set; }

        IClient Client { get; set; }

        DateTime DateExpiresUtc { get; set; }
    }

    public interface IUserRefreshTokenKey
    {
        Guid UserId { get; set; }

        Guid ClientId { get; set; }
    }
}