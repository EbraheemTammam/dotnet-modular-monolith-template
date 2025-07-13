using Microsoft.Extensions.DependencyInjection;

namespace Base.Interfaces;

public interface IModuleRegistrar
{
    void Register(IServiceCollection services);
}
