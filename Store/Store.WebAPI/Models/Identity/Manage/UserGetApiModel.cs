using System;

namespace Store.WebAPI.Models.Identity
{
    public class UserGetApiModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsApproved { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public string[] Roles { get; set; }
    }
}