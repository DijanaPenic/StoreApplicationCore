using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserRefreshToken : IPoco
    {
        string Value { get; set; }

        Guid UserId { get; set; }

        Guid ClientId { get; set; }

        DateTime DateExpiresUtc { get; set; }
    }
}