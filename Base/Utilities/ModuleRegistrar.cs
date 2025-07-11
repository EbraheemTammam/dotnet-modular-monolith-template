using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Base.Interfaces;
using Base.Commands;
using Base.Models;
using Base.Repositories;
using Base.Data;

namespace Base.Utilities;

public class BaseModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<BaseDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<MakeMigrationsCommand>();
        services.AddScoped<StartAppCommand>();

        services.AddScoped<IRepository<Document>, GenericRepository<BaseDbContext, Document>>();
    }
}
