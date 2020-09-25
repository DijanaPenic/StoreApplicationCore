using System;

using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class ExternalLogin //: IExternalLogin
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid UserId { get; set; }

        public IIdentityUser User { get; set; }
    }
}