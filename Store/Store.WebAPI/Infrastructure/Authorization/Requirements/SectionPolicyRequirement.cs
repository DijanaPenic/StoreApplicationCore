using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;

namespace Store.WebAPI.Infrastructure.Authorization.Requirements
{
    public class SectionPolicyRequirement : IAuthorizationRequirement
    {
        public SectionType SectionType { get; set; }

        public AccessType AccessAction { get; set; }

        public SectionPolicyRequirement(SectionType sectionType, AccessType accessAction)
        {
            SectionType = sectionType;
            AccessAction = accessAction;
        }
    } 
}