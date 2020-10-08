using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserClaim : IClaimBase
    {
        public Guid UserId { get; set; }
    }
}