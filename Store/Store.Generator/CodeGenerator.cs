using DotLiquid;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Store.Generator.Models;

namespace Store.Generator
{
    internal static class CodeGenerator
    {
        internal async static Task RunSqlQueriesAsync()
        {
            string roothPath = @"..\..\..\..\Store.Repository";

            string sourcePath = Path.Combine(roothPath, "SqlQueries");
            string outputPath = Path.Combine(roothPath, "SqlQueries");

            string[] directories = Directory.GetDirectories(sourcePath);
            IList<DirectoryModel> directoryModels = new List<DirectoryModel>();

            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                DirectoryModel directoryModel = new DirectoryModel
                {
                    Name = directoryInfo.Name,
                    Files = new List<FileModel>()
                };

                string[] files = Directory.GetFiles(directoryInfo.FullName, "*.sql", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    directoryModel.Files.Add(new SqlFileModel()
                    {
                        Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                        RelativePath = Path.GetRelativePath(roothPath, fileInfo.FullName),
                        FullPath = fileInfo.FullName,
                        Parameters = GetSqlQueryParameters(fileInfo.FullName)
                    });
                }

                directoryModels.Add(directoryModel);
            }

            // Generate sql query .cs classes
            CreateDirectoryIfNotExist(outputPath);

            // SQL Queries
            Template sqlQueriesTemplate = Template.Parse(await LoadTemplateAsync("SqlQueries"));
            string sqlQueriesResult = sqlQueriesTemplate.Render(Hash.FromAnonymousObject(new 
            { 
                SqlQueryCategories = directoryModels
            }));
            await File.WriteAllTextAsync(Path.Combine(outputPath, "SqlQueries.generated.cs"), sqlQueriesResult);
        }

        private static SqlParameterModel[] GetSqlQueryParameters(string path)
        {
            string commandText = File.ReadAllText(path);

            Regex rgxExpression = new Regex(@"\@([^=<>\s\']+)");
            SqlParameterModel[] parameters = rgxExpression.Matches(commandText)
                                                          .Select(p => p.Value.TrimStart('(', '@').TrimEnd(')'))
                                                          .Distinct()
                                                          .Select(p => new SqlParameterModel { Name = p })
                                                          .ToArray();

            return parameters;
        }

        private static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static Task<string> LoadTemplateAsync(string name)
        {
            return File.ReadAllTextAsync(Path.Combine(@"..\..\..\Templates", $"{name}.liquid"));
        }
    }
}