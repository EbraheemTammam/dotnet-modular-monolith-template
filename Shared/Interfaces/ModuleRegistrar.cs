using Microsoft.Extensions.DependencyInjection;

namespace Shared.Interfaces;

public interface IModuleRegistrar
{
    void Register(IServiceCollection services);
}
