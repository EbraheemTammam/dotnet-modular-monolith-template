using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Base.Interfaces;
using Base.Commands;
using Base.Data;
using Base.Services;

namespace Base.Utilities;

internal class BaseModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddDbContextPool<BaseDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));

        services.AddScoped<MakeMigrationsCommand>();
        services.AddScoped<StartAppCommand>();

        services.AddScoped<INotificationService, TwilioService>();
    }
}
