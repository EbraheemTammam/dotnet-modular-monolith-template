using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Base.Interfaces;
using Auth.Interfaces;
using Auth.Services;
using Auth.Data;

namespace Auth.Utilities;

public class AuthModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddDbContextPool<AuthDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));

        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<ICurrentLoggedInUser, CurrentLoggedInUser>();

        services.AddScoped<IExtendedUserService, ExtendedUserService>();
    }
}
