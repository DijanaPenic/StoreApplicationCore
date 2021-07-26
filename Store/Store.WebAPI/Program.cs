using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Store.WebAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    
                    config.AddEnvironmentVariables(prefix: "StoreApp_");
                    
                    // Azure Key Vault for applications that are not hosted in Azure
                    // Use for local environment if you decide to move secrets from environment variables to Azure kv.
                    
                    // if (context.HostingEnvironment.IsDevelopment())
                    // {
                    //     IConfigurationRoot builtConfig = config.Build();
                    //
                    //     using X509Store store = new(StoreLocation.CurrentUser);
                    //     store.Open(OpenFlags.ReadOnly);
                    //     
                    //     X509Certificate2Collection certs = store.Certificates.Find
                    //     (
                    //         X509FindType.FindByThumbprint,
                    //         builtConfig["AzureADCertThumbprint"], 
                    //         false
                    //     );
                    //
                    //     config.AddAzureKeyVault
                    //     (
                    //         new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
                    //         new ClientCertificateCredential
                    //         (
                    //             builtConfig["AzureADDirectoryId"], 
                    //             builtConfig["AzureADApplicationId"], 
                    //             certs.OfType<X509Certificate2>().Single()
                    //         ),
                    //         new KeyVaultSecretManager()
                    //     );
                    //
                    //     store.Close();
                    // }
                    
                    // Azure Key Vault for applications that are hosted in Azure
                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        IConfigurationRoot builtConfig = config.Build();
                        SecretClient secretClient = new
                        (
                            new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
                            new DefaultAzureCredential()
                        );
                        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, serverOptions) =>
                    {
                        serverOptions.Configure(context.Configuration.GetSection("Kestrel"));
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}