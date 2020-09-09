using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities.Identity
{
    public class ClaimEntity
    {
        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }
    }
}