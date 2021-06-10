using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserRefreshToken : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        string Value { get; set; }

        Guid UserId { get; set; }

        Guid ClientId { get; set; }

        IClient Client { get; set; }

        DateTime DateExpiresUtc { get; set; }
    }
}