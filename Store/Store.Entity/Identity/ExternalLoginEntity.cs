using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities.Identity
{
    public class ExternalLoginEntity
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }
    }
}