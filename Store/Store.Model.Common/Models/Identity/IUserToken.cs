using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserToken
    {
        string Value { get; set; }

        Guid UserId { get; set; }

        string LoginProvider { get; set; }

        string Name { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}