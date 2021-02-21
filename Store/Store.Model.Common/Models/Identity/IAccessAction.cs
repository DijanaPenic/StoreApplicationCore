using Store.Common.Enums;

namespace Store.Model.Common.Models.Identity
{
    public interface IAccessAction
    {
        AccessType Type { get; set; }

        bool IsEnabled { get; set; }
    }
}