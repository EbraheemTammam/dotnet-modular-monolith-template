using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Interfaces;

public interface IModuleRegistrar
{
    void Register(IServiceCollection services, IConfiguration configuration);
}
