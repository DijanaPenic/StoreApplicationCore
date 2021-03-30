using Store.Common.Enums;

namespace Store.Common.Parameters.Filtering
{
    public class PermissionFilteringParameters : FilteringParameters, IPermissionFilteringParameters
    {
        public SectionType SectionType { get; set; }
    }
}