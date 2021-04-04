using System;

namespace Store.WebAPI.Models.Identity
{
    public class UserGetApiModel : ApiModelBase
    {
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsApproved { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public string[] Roles { get; set; }
    }
}