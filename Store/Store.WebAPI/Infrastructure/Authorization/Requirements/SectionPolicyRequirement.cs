using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Requirements
{
    public class SectionPolicyRequirement : IAuthorizationRequirement
    {
        public SectionType SectionType { get; set; }

        public AccessAction AccessAction { get; set; }

        public SectionPolicyRequirement(SectionType sectionType, AccessAction accessAction)
        {
            SectionType = sectionType;
            AccessAction = accessAction;
        }
    } 
}