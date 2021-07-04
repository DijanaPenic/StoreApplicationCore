using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DotLiquid;

using static System.String;

using Store.Entities;
using Store.Generator.Models;
using Store.Common.Extensions;

namespace Store.Generator
{
    internal static class CodeGenerator
    {
        internal static async Task RunEntitySchemasAsync()
        {
            // Configure paths
            const string entityPath = @"..\..\..\..\Store.Entity";
            const string databaseAccessLayerPath = @"..\..\..\..\Store.DAL";
            
            string outputPath = Path.Combine(databaseAccessLayerPath, "Schema");

            // Retrieve entity files
            string[] files = Directory.GetFiles(entityPath, "*Entity.cs", SearchOption.AllDirectories)
                .Where(d => !d.Contains("Core")).ToArray();

            // Purge schema directory first and then create empty one
            Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);
            
            // Load entity schema template
            Template entitySchemaTemplate = Template.Parse(await LoadTemplateAsync("EntitySchema"));
            
            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                
                // Get entity name
                string entityName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                
                // Get entity namespace
                string entityNamespace = (await File.ReadAllLinesAsync(fileInfo.FullName))
                    .FirstOrDefault(l => l.Contains("namespace"))
                    ?.Replace("namespace", Empty)
                    .Trim();

                // Get entity type
                Type entityType = typeof(IDbBaseEntity).Assembly.GetType($"{entityNamespace}.{entityName}");

                // Get parent directory name, i.e. schema name
                string parentDirectory =  Path.GetFileName(fileInfo.DirectoryName)
                    ?.Replace("Store.Entity", Empty); 
                
                // Get schema class name
                string className = entityName.Replace("Entity", Empty);

                EntityModel entityModel = new()
                {
                    ClassName = className,
                    Properties = GetProperties(entityType),
                    TableName = GetTableName(entityName, parentDirectory),
                    ClassNamespace = GetSchemaNamespace(parentDirectory)
                };

                string schemaContent = entitySchemaTemplate.Render(Hash.FromAnonymousObject(new { Entity = entityModel }));
                
                string schemaDirectory = Path.Combine(outputPath, parentDirectory ?? Empty);
                CreateDirectoryIfNotExist(schemaDirectory);
                
                string schemaPath = Path.Combine(schemaDirectory, $"{className}Schema.generated.cs");
                
                await File.WriteAllTextAsync(schemaPath, schemaContent);
            }
        }

        private static string GetSchemaNamespace(string parentDirectory)
        {
            const string relativeSchemaPath = "Store.DAL.Schema";
            
            return (IsNullOrEmpty(parentDirectory)) ? relativeSchemaPath : $"{relativeSchemaPath}.{parentDirectory}";
        }
        
        private static string GetTableName(string entityName, string parentDirectory)
        {
            string schemaName = parentDirectory.ToSnakeCase();
            entityName = entityName.Replace("Entity", Empty).ToSnakeCase();

            return (IsNullOrEmpty(schemaName)) ? entityName : $"{schemaName}.{entityName}";
        }

        private static IEnumerable<PropertyModel> GetProperties(IReflect entityType)
        {
            PropertyInfo[] propertyInfos = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                yield return new PropertyModel()
                {
                    Name = propertyInfo.Name
                };
            }
        }
        
        internal static async Task RunSqlQueriesAsync()
        {
            const string rootPath = @"..\..\..\..\Store.Repository";

            string sourcePath = Path.Combine(rootPath, "SqlQueries");
            string outputPath = Path.Combine(rootPath, "SqlQueries");

            string[] directories = Directory.GetDirectories(sourcePath);
            IList<DirectoryModel> directoryModels = new List<DirectoryModel>();

            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new(directory);

                DirectoryModel directoryModel = new()
                {
                    Name = directoryInfo.Name,
                    Files = new List<FileModel>()
                };

                string[] files = Directory.GetFiles(directoryInfo.FullName, "*.sql", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new(file);

                    directoryModel.Files.Add(new SqlQueryModel()
                    {
                        Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                        RelativePath = Path.GetRelativePath(rootPath, fileInfo.FullName),
                        FullPath = fileInfo.FullName,
                        Parameters = await GetSqlQueryParametersAsync(fileInfo.FullName)
                    });
                }

                directoryModels.Add(directoryModel);
            }

            // Generate sql query .cs classes
            CreateDirectoryIfNotExist(outputPath);

            // SQL Queries
            Template sqlQueriesTemplate = Template.Parse(await LoadTemplateAsync("SqlQueries"));
            string sqlQueriesContent = sqlQueriesTemplate.Render(Hash.FromAnonymousObject(new 
            { 
                SqlQueryCategories = directoryModels
            }));
            await File.WriteAllTextAsync(Path.Combine(outputPath, "SqlQueries.generated.cs"), sqlQueriesContent);
        }

        private static async Task<SqlParameterModel[]> GetSqlQueryParametersAsync(string path)
        {
            string commandText = await File.ReadAllTextAsync(path);

            Regex rgxExpression = new(@"\@([^=<>\s\']+)");
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