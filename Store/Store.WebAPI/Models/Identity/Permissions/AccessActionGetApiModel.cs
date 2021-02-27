using System;

namespace Store.WebAPI.Models.Identity
{
    public class AccessActionGetApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}