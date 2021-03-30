namespace Store.Repository.Repositories.Models
{
    public class FilterModel : IFilterModel
    {
        public string Expression { get; set; }

        public dynamic Parameters { get; set; }
    }
}