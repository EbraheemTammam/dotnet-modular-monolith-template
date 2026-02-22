using Microsoft.Extensions.DependencyInjection;

using Shared.Interfaces;
using Shared.Commands;

namespace Shared.Utilities;

internal class SharedModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<MakeMigrationsCommand>();
        services.AddScoped<StartAppCommand>();
    }
}
