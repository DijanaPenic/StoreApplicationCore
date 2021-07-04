using System;
using System.Threading.Tasks;

namespace Store.Generator
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Code generation has started!");

            await CodeGenerator.RunSqlQueriesAsync();
            await CodeGenerator.RunEntitySchemasAsync();

            Console.WriteLine("Code generation has finished!");
        }
    }
}