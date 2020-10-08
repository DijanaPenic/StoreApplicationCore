using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class RoleClaim : ClaimBase, IRoleClaim
    {
        public Guid RoleId { get; set; }
    }
}