using System;

namespace Store.Entities
{
    public interface IDBBaseEntity
    {
        DateTime DateCreatedUtc { get; set; }
    }
}