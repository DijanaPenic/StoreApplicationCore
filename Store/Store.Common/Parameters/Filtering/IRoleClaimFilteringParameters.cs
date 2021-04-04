using System;

namespace Store.Common.Parameters.Filtering
{
    public interface IRoleClaimFilteringParameters : IFilteringParameters
    {
        string Type { get; set; }

        Guid? RoleId { get; set; }
    }
}