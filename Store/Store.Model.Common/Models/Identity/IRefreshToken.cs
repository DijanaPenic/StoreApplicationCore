using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    // TODO - need to fix
    public interface IRefreshToken : IPoco
    {
        string Value { get; set; }

        Guid UserId { get; set; }

        IIdentityUser User { get; set; }

        Guid ClientId { get; set; }

        IClient Client { get; set; }

        string ProtectedTicket { get; set; }

        DateTime ExpiresUTC { get; set; }
    }
}