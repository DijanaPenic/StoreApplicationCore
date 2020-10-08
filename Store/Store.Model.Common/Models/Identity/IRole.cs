using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IRole : IPoco
    {
        string ConcurrencyStamp { get; set; }

        string Name { get; set; }

        string NormalizedName { get; set; }
    }
}