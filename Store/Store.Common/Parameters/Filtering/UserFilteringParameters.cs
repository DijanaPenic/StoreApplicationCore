namespace Store.Common.Parameters.Filtering
{
    public class UserFilteringParameters : FilteringParameters, IUserFilteringParameters
    {
        public bool ShowInactive { get; set; }
    }
}