using Store.Common.Enums;
using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IClient : IPoco
    {
        string Name { get; set; }

        string Secret { get; set; }

        string Description { get; set; }

        ApplicationType ApplicationType { get; set; }

        bool Active { get; set; }

        int AccessTokenLifeTime { get; set; }

        int RefreshTokenLifeTime { get; set; }

        string AllowedOrigin { get; set; }
    }
}