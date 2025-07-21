using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using Base.Interfaces;
using Accounts.Data;
using Accounts.Models;
using Accounts.Commands;
using Accounts.Interfaces;
using Accounts.Services;

namespace Accounts.Utilities;

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

        services.AddAuthentication();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        services.AddScoped<CreateSuperUserCommand>();

        services.AddScoped<VerificationManager>();

        services.AddScoped<HttpContextAccessor>();

        services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<IJWTAuthService, TokenService>();
    }
}
