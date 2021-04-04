using System;

namespace Store.Common.Parameters.Filtering
{
    public class RoleClaimFilteringParameters : FilteringParameters, IRoleClaimFilteringParameters
    {
        public string Type { get; set; }

        public Guid? RoleId { get; set; }
    }
}