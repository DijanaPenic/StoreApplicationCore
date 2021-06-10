using System;

namespace Store.Entities
{
    public interface IDBChangable
    {
        DateTime DateUpdatedUtc { get; set; }
    }
}