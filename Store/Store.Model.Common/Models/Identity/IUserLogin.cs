using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserLogin
    {
        string ProviderDisplayName { get; set; }

        Guid UserId { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}