using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;

namespace Store.WebAPI.Infrastructure.Authorization.Attributes
{
    public class SectionAuthorizationAttribute : AuthorizeAttribute 
    {
        private const string PolicyPrefix = "Section_";

        public SectionAuthorizationAttribute(SectionType sectionType, AccessType accessAction) 
            => Policy = $"{PolicyPrefix}{sectionType}.{accessAction}";
    }
}