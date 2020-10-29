using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Store.WebAPI.Application.Startup
{
    public static class SwaggerExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Store API",
                    Description = "API documentation for Store application."
                });

                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swaggerOptions.IncludeXmlComments(xmlPath);
            });
        }
    }
}
