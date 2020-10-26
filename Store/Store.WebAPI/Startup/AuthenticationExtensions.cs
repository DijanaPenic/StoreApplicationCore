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
            services.AddIdentityCore<IUser>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequireUppercase = true;
                identityOptions.Password.RequireNonAlphanumeric = true;

                identityOptions.Lockout.AllowedForNewUsers = true;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                identityOptions.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddRoles<IRole>()
            .AddSignInManager<SignInManager<IUser>>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoleManager<ApplicationRoleManager>()
            .AddDefaultTokenProviders();

            services.AddTransient<ApplicationAuthManager>();

            // JWT cnfiguration
            JwtTokenConfig jwtTokenConfig = authConfiguration.Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.RequireHttpsMetadata = true;
                jwtOptions.SaveToken = true;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        }
    }
}