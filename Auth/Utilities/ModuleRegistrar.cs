using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

using Base.Interfaces;
using Users.Models;
using Auth.Interfaces;
using Auth.Services;
using Auth.Data;

namespace Auth.Utilities;

internal class AuthModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddDbContextPool<AuthDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));

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

        services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();
        services.AddScoped<ICurrentLoggedInUser, CurrentLoggedInUser>();

        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<IJWTAuthService, TokenService>();
        services.AddScoped<IExtendedUserService, ExtendedUserService>();

    }
}
