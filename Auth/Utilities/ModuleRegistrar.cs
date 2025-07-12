using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Base.Interfaces;
using Auth.Interfaces;
using Auth.Services;
using Auth.Data;

namespace Auth.Utilities;

public class AuthModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<AuthDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<ICurrentLoggedInUser, CurrentLoggedInUser>();

        services.AddScoped<IExtendedUserService, ExtendedUserService>();
    }
}
