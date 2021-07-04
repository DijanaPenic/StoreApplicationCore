using System;

namespace Store.Entities
{
    public interface IDbBaseEntity
    {
        DateTime DateCreatedUtc { get; set; }
    }
}