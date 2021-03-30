namespace Store.Common.Parameters.Filtering
{
    public interface IUserFilteringParameters : IFilteringParameters
    {
        bool ShowInactive { get; set; }
    }
}