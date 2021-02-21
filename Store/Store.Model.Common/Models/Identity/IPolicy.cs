using Store.Common.Enums;

namespace Store.Model.Common.Models.Identity
{
    public interface IPolicy
    {
        IAccessAction[] Actions { get; set; }

        SectionType Section { get; set; }
    }
}