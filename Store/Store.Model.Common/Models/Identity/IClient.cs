using System;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IClient : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Secret { get; set; }

        string Description { get; set; }

        ApplicationType ApplicationType { get; set; }

        bool Active { get; set; }

        int AccessTokenLifeTime { get; set; }

        int RefreshTokenLifeTime { get; set; }

        string AllowedOrigin { get; set; }

        ICollection<IUserRefreshToken> RefreshTokens { get; set; }

        ICollection<IEmailTemplate> EmailTemplates { get; set; }
    }
}