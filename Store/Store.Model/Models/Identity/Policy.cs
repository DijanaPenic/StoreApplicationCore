using Store.Common.Enums;
using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class Policy : IPolicy
    {
        public IAccessAction[] Actions { get; set; }

        public SectionType Section { get; set; }
    }
}