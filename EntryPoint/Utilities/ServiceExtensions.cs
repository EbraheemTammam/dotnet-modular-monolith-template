using Base.Interfaces;
using Auth.Models;
using Auth.Utilities;

namespace EntryPoint.Utilities;

public static class ServiceExtensions
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        var cookieSettingSection = configuration.GetSection("CookieSetting");
        services.Configure<CookieSetting>(cookieSettingSection);

        services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen();
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddCorsConfiguration(configuration);
        services.AddIISIntegrationConfiguration();
        services.AddControllers();
        services.AddHttpContextAccessor();

        return services;
    }

    public static void RegisterModules(this IServiceCollection services, IConfiguration configuration, string[] modules)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(modules, nameof(modules));

        foreach (string module in modules)
        {
            try
            {
                string typeName = $"{module}.Utilities.{module}ModuleRegistrar, {module}";
                Type? registrarType = Type.GetType(typeName);

                if (registrarType == null || !typeof(IModuleRegistrar).IsAssignableFrom(registrarType))
                {
                    Console.Error.WriteLine($"Module {module}: IModuleRegistrar not found.");
                    continue;
                }

                IModuleRegistrar? registrar = Activator.CreateInstance(registrarType) as IModuleRegistrar;
                if (registrar == null)
                {
                    Console.Error.WriteLine($"Module {module}: Failed to create IModuleRegistrar instance.");
                    continue;
                }

                registrar.Register(services, configuration);
                Console.WriteLine($"Module {module} registered successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Module {module} could not be registered: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder.WithOrigins(configuration.GetSection("CookieSetting").Get<CookieSetting>()!.AllowedOrigins!)
                                      .AllowCredentials()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                );
            }
        );

    public static IServiceCollection AddIISIntegrationConfiguration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });
}
