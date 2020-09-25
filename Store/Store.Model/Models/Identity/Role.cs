using System;
using System.Collections.Generic;

using Store.Common.Helpers;
using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class Role //: IIdentityRole
    {
        public Role()
        {
            Id = GuidHelper.NewSequentialGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public ICollection<IIdentityUser> Users { get; set; }
    }
}