using System;

namespace Store.Models.Api.Identity
{
    public class RoleApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Stackable { get; set; }
    }
}