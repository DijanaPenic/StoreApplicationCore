using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserLogin : IUserLoginKey
    {
        string ProviderDisplayName { get; set; }

        string Token { get; set; }

        bool IsConfirmed { get; set; }

        Guid UserId { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }

    public interface IUserLoginKey
    {
        string LoginProvider { get; set; }

        string ProviderKey { get; set; }
    }
}