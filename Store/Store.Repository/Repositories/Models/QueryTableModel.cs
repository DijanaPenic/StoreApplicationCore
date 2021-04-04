using Store.Repository.Common.Models;

namespace Store.Repository.Repositories.Models
{
    internal class QueryTableModel : IQueryTableModel
    {
        public string TableName { get; set; }
         
        public string TableAlias { get; set; }
         
        public string SelectStatement { get; set; }

        public string IncludeStatement { get; set; }
    }
}