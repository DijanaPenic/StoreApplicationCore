using System;

namespace Store.Models.Api
{
    public class BaseApiModel
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}