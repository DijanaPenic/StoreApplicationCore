namespace Store.Repository.Repositories.Models
{
    public interface IFilterModel
    {
        string Expression { get; set; }

        dynamic Parameters { get; set; }
    }
}