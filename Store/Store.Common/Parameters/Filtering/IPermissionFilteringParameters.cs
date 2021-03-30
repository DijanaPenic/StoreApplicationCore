using Store.Common.Enums;

namespace Store.Common.Parameters.Filtering
{
    public interface IPermissionFilteringParameters : IFilteringParameters
    {
        SectionType SectionType { get; set; }
    }
}