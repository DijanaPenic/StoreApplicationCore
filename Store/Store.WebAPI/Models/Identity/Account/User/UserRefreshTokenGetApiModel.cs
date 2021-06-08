using System;

namespace Store.WebAPI.Models.Identity
{
    public class UserRefreshTokenGetApiModel : ApiModelBase
    {
        public string Value { get; set; }

        public Guid ClientId { get; set; }

        public DateTime DateExpiresUtc { get; set; }
    }
}