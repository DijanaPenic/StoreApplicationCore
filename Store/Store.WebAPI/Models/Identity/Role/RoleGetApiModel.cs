using System;
using System.Collections.Generic;

namespace Store.WebAPI.Models.Identity
{
    public class RoleGetApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public IEnumerable<RoleClaimGetApiModel> Claims { get; set; }
    }
}