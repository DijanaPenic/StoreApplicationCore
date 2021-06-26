using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerServices(this IServiceCollection services)
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

        public static void AddSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(swaggerOptions =>
            {
                swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
                swaggerOptions.RoutePrefix = string.Empty;
            });
        }
    }
}
