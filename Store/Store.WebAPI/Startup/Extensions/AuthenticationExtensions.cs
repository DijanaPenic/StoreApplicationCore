using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Service.Options;
using Store.Service.Constants;
using Store.Services.Identity;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Infrastructure.Authentication.Handlers;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure token lifespan
            // Note: Token example - token for password reset
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1); // Sets the expiry to one day
            });

            // Identity configuration
            // Note: "AddIdentity" shouldn't be used if we want fine grain control over auth. 
            // Note: "AddIdentityCore" will setup identity without auth, then we can call AddAuthentication() and configure things exactly how we want.
            services.AddIdentityCore<IUser>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequireUppercase = true;
                identityOptions.Password.RequireNonAlphanumeric = true;

                identityOptions.Lockout.AllowedForNewUsers = true;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                identityOptions.Lockout.MaxFailedAccessAttempts = 3;

                identityOptions.User.RequireUniqueEmail = true;

                identityOptions.SignIn.RequireConfirmedEmail = true;
                identityOptions.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddRoles<IRole>()
            //.AddSignInManager<ApplicationSignInManager>()   // ApplicationSignInManager doesn't derive from SignInManager<IUser> so cannot register service using the AddSignInManager method
            .AddUserManager<ApplicationUserManager>()       // Scoped - TODO - need to confirm
            .AddRoleManager<ApplicationRoleManager>()       // Scoped
            .AddDefaultTokenProviders();

            // ApplicationSignInManager dependencies
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ApplicationSignInManager>();

            // ApplicationAuthManager dependencies
            services.AddTransient<ApplicationAuthManager>();

            // ApplicationPermissionsManager dependencies
            services.AddTransient<ApplicationPermissionsManager>();

            // Cookies configuration
            // Caution: need to enable cookies because of SignInManager: "All the authentication logic is tied to sign in manager which is tied to cookies in general."
            // Source: https://github.com/openiddict/openiddict-core/issues/578

            // Set cookie authentication options
            static void cookieAuthOptions(CookieAuthenticationOptions options)
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                // Use 401 status code instead of Login redirect which is used by default
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            }

            // Set cookies
            services.AddAuthentication()
            .AddCookie(ApplicationIdentityConstants.AccountVerificationScheme, cookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, cookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, cookieAuthOptions)
            .AddCookie(IdentityConstants.ExternalScheme, cookieAuthOptions);

            // Two-factor configuration
            services.Configure<TwoFactorAuthOptions>(configuration.GetSection(TwoFactorAuthOptions.Position));

            // JWT configuration
            IConfigurationSection jwtTokenSection = configuration.GetSection(JwtTokenOptions.Position);

            JwtTokenOptions jwtTokenConfig = jwtTokenSection.Get<JwtTokenOptions>();
            services.Configure<JwtTokenOptions>(jwtTokenSection);

            // Note: Web API will be used as authentication and resource server - it will issue and validate incoming tokens.
            // Note: The following configuration is used to validate incoming access (JWT) tokens.
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,     // Issuer will be checked by the client (web application)
                ValidateIssuerSigningKey = true,    // TODO - can I disable this?
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                ValidateAudience = true,    // Audience is checked by the resource server (in this project: Web API)
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
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            // Add custom authentication scheme
            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, ClientAuthenticationHandler>("ClientAuthenticationScheme", options => { });

            // External Login configuration
            services.ConfigureExternalProviders(configuration);
        }
    }
}