using System;

namespace Store.Entities
{
    public interface IDbChangeable
    {
        DateTime DateUpdatedUtc { get; set; }
    }
}