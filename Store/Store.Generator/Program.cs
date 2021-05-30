using System;
using System.Threading.Tasks;

namespace Store.Generator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Code generation has started!");

            await CodeGenerator.RunSqlQueriesAsync();

            Console.WriteLine("Code generation has finished!");
        }
    }
}