namespace Store.Repository.Repositories.Models
{
    public interface IQueryTableModel
    {
        string TableName { get; set; }

        string TableAlias { get; set; }

        string SelectStatement { get; set; }

        string IncludeStatement { get; set; }
    }
}