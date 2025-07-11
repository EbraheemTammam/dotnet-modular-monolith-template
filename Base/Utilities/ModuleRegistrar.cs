using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Base.Interfaces;
using Base.Commands;

namespace Base.Utilities;

public class BaseModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<MakeMigrationsCommand>();
        services.AddScoped<StartAppCommand>();
    }
}
