using System;

namespace Store.WebAPI.Models.Identity
{
    public class UserTokenGetApiModel
    {
        public string Value { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }
    }
}