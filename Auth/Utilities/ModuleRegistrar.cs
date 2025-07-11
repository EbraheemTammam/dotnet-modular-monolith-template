using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Base.Interfaces;
using Auth.Interfaces;
using Auth.Services;

namespace Auth.Utilities;

public class AuthModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<ICurrentLoggedInUser, CurrentLoggedInUser>();
    }
}
