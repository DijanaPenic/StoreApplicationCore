using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserToken : IUserTokenKey
    {
        string Value { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }

    public interface IUserTokenKey
    {
        Guid UserId { get; set; }

        string LoginProvider { get; set; }

        string Name { get; set; }
    }
}