using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using Shared.Interfaces;
using Accounts.Data;
using Accounts.Models;
using Accounts.Commands;
using Accounts.Interfaces;
using Accounts.Services;
using Accounts.Hashers;
using Accounts.Settings;

namespace Accounts;

internal class AccountsModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddDbContextPool<AccountsDbContext>(
            options => options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING"))
        );

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AccountsDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = bool.Parse(Environment.GetEnvironmentVariable("JWT_VALIDATE_ISSUER")!),
                ValidateAudience = bool.Parse(Environment.GetEnvironmentVariable("JWT_VALIDATE_AUDIENCE")!),
                ValidateLifetime = bool.Parse(Environment.GetEnvironmentVariable("JWT_VALIDATE_LIFETIME")!),
                ValidateIssuerSigningKey = bool.Parse(Environment.GetEnvironmentVariable("JWT_VALIDATE_ISSUER_SIGNING_KEY")!),
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY")!)
                )
            };
        });
        services.AddAuthorization();

        // cli
        services.AddScoped<CreateSuperUserCommand>();
        // settings
        services.AddSingleton<JwtOptions>();
        // internals
        services.AddScoped<HttpContextAccessor>();
        // hashers
        services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();
        // services
        services.AddScoped<IAuthService, JWTService>();
    }
}
