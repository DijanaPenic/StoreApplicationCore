using Store.Common.DataTypes;

namespace Store.Common.Parameters.Filtering
{
    public interface IFilteringParameters
    {
        DateTimeRange? DateCreated { get; set; }

        DateTimeRange? DateUpdated { get; set; }

        string SearchPhrase { get; set; }
    }
}