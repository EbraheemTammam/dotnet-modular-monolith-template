using StackExchange.Redis;

using Base.Interfaces;

namespace EntryPoint.Utilities;

public static class ServiceExtensions
{
    public static IServiceCollection Configure(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddCorsConfiguration();
        services.AddIISIntegrationConfiguration();
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddRedisConfiguration();

        return services;
    }

    public static void RegisterModules(this IServiceCollection services, string[] modules)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
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

                registrar.Register(services);
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

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services) =>
        services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder.WithOrigins(Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")!.Split(","))
                                      .AllowCredentials()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                );
            }
        );

    public static IServiceCollection AddIISIntegrationConfiguration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });

    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services) =>
        services.AddSingleton<IConnectionMultiplexer>(opt =>
        {
            var connection = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            return ConnectionMultiplexer.Connect(connection!);
        });
}
