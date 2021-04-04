namespace Store.Repository.Common.Models
{
    public interface IQueryTableModel
    {
        string TableName { get; set; }

        string TableAlias { get; set; }

        string SelectStatement { get; set; }

        string IncludeStatement { get; set; }
    }
}