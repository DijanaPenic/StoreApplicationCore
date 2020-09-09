using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities.Identity
{
    [Table("RefreshToken")]

    public class RefreshTokenEntity : IDBPoco
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Value { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey("Client")]
        public Guid ClientId { get; set; }

        public ClientEntity Client { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }
    }
}