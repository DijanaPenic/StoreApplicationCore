using System;

namespace Store.Model.Common.Models.Core
{
    public interface IPoco
    {
        Guid Id { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}