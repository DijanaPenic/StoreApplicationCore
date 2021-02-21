using Store.Common.Enums;
using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class AccessAction : IAccessAction
    {
        public AccessType Type { get; set; }

        public bool IsEnabled { get; set; }
    }
}