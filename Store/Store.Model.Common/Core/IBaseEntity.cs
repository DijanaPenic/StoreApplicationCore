using System;

namespace Store.Model.Common.Models.Core
{
    public interface IBaseEntity
    {
        DateTime DateCreatedUtc { get; set; }
    }
}