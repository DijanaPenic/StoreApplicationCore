using System;
using System.Collections.Generic;

namespace Store.Model.Common.Models.Identity
{
    // TODO - need to fix
    public interface IIdentityRole 
    {
        // Need to override Id from the IUser so that AutoMapper can map the Id property (we need setter)
        //new Guid Id { get; set; }

        bool Stackable { get; set; }

        ICollection<IIdentityUser> Users { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}
