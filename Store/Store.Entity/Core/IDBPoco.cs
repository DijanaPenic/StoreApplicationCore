using System;

namespace Store.Entities
{
    public interface IDBPoco
    {
        Guid Id { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}