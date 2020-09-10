using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IExternalLogin
    {
        string LoginProvider { get; set; }

        string ProviderKey { get; set; }

        Guid UserId { get; set; }

        IIdentityUser User { get; set; }
    }
}
