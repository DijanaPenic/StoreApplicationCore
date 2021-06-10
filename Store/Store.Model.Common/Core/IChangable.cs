using System;

namespace Store.Model.Common.Models.Core
{
    public interface IChangable
    {
        DateTime DateUpdatedUtc { get; set; }
    }
}