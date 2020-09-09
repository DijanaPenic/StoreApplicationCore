using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Store.Common.Enums;

namespace Store.Entities.Identity
{
    [Table("Client")]
    public class ClientEntity : IDBPoco
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Client Description cannot be longer than 100 characters.")]
        public string Description { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        [StringLength(100, ErrorMessage = "Allowed Origin cannot be longer than 100 characters.")]
        public string AllowedOrigin { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}