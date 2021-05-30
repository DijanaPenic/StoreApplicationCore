using System.IO;

namespace Store.Repository.Queries
{
    public static partial class SqlQueries
    {
        private static string GetSqlCommandText(string relativePath)
        {
            return File.ReadAllText(Path.Combine(@"..\Store.Repository", relativePath));
        }
    }
}