using System;

using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class RefreshToken //: IRefreshToken
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public Guid UserId { get; set; }

        public IIdentityUser User { get; set; }

        public Guid ClientId { get; set; }

        public IClient Client { get; set; }

        public string ProtectedTicket { get; set; }

        public DateTime ExpiresUTC { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}