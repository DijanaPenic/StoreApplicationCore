using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Attributes
{
    public class SectionAuthorizationAttribute : AuthorizeAttribute 
    {
        const string POLICY_PREFIX = "Section_";

        public SectionAuthorizationAttribute(SectionType sectionType, AccessAction accessAction) => Policy = $"{POLICY_PREFIX}{sectionType}.{accessAction}";
    }
}