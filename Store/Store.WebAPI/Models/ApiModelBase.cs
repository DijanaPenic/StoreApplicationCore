using System;

namespace Store.WebAPI.Models
{
    public class ApiModelBase
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}