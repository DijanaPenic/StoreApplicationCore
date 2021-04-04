namespace Store.Repository.Common.Models
{
    public interface IFilterModel
    {
        string Expression { get; set; }

        dynamic Parameters { get; set; }
    }
}