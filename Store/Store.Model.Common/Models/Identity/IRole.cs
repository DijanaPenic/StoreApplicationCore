using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IRole : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        string ConcurrencyStamp { get; set; }

        string Name { get; set; }

        string NormalizedName { get; set; }

        bool Stackable { get; set; }

        ICollection<IRoleClaim> Claims { get; set; }
    }
}