using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Models;
using Store.WebAPI.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Application.Startup
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IConfigurationSection authConfiguration)
        {
            // Identity configuration
            services.AddIdentity<IUser, IRole>(identityOptions =>
                    {
                        identityOptions.Password.RequiredLength = 8;
                        identityOptions.Password.RequireDigit = true;
                        identityOptions.Password.RequireUppercase = true;
                        identityOptions.Password.RequireNonAlphanumeric = true;

                        identityOptions.Lockout.AllowedForNewUsers = true;
                        identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                        identityOptions.Lockout.MaxFailedAccessAttempts = 3;
                    })
                    .AddUserManager<ApplicationUserManager>()   // Scoped
                    .AddRoleManager<ApplicationRoleManager>()   // Scoped
                    .AddDefaultTokenProviders();                

            services.AddTransient<ApplicationAuthManager>();   

            // JWT cnfiguration
            JwtTokenConfig jwtTokenConfig = authConfiguration.Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);

            // Note: Web API will be used as authentication and resource server - it will issue and validate incoming tokens
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,     // Issuer will be checked by the client (web application)
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                ValidateAudience = true,    // Audience is checked by the resource server (in this application: Web API)
                ValidAudience = jwtTokenConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(authOptions =>
                    {
                        authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(jwtOptions =>
                    {
                        jwtOptions.RequireHttpsMetadata = true;
                        jwtOptions.SaveToken = true;
                        jwtOptions.TokenValidationParameters = tokenValidationParameters;
                    });
        }
    }
}