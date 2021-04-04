using Store.Repository.Common.Models;

namespace Store.Repository.Repositories.Models
{
    internal class FilterModel : IFilterModel
    {
        public string Expression { get; set; }

        public dynamic Parameters { get; set; }
    }
}