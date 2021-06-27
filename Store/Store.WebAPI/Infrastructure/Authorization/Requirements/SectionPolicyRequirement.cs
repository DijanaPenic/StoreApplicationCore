using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;

namespace Store.WebAPI.Infrastructure.Authorization.Requirements
{
    public class SectionPolicyRequirement : IAuthorizationRequirement
    {
        public SectionType SectionType { get; }

        public AccessType AccessAction { get; }

        public SectionPolicyRequirement(SectionType sectionType, AccessType accessAction)
        {
            SectionType = sectionType;
            AccessAction = accessAction;
        }
    } 
}