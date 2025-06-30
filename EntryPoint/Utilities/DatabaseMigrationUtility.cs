using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntryPoint.Utilities;

public static class DatabaseMigrationUtility
{
    public static void MigrateDatabase(this IServiceProvider serviceProvider, string assemblyName)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            ArgumentNullException.ThrowIfNullOrEmpty(assemblyName, nameof(assemblyName));

            string dbContextTypeName = $"{assemblyName}.Data.{assemblyName}DbContext";
            try
            {
                string fullyQualifiedTypeName = $"{dbContextTypeName}, {assemblyName}";
                Type? dbContextType = Type.GetType(fullyQualifiedTypeName);

                if (dbContextType == null)
                {
                    throw new InvalidOperationException($"DbContext type '{dbContextTypeName}' not found in assembly '{assemblyName}'.");
                }

                if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new InvalidOperationException($"Type '{dbContextTypeName}' is not a DbContext.");
                }

                using var scope = serviceProvider.CreateScope();

                object? dbContext = scope.ServiceProvider.GetService(dbContextType);
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext '{dbContextTypeName}' is not registered in the DI container.");
                }

                PropertyInfo? databaseProperty = dbContextType.GetProperty("Database", BindingFlags.Public | BindingFlags.Instance);
                if (databaseProperty == null)
                {
                    throw new InvalidOperationException($"DbContext '{dbContextTypeName}' does not have a Database property.");
                }

                object? databaseFacade = databaseProperty.GetValue(dbContext);
                if (databaseFacade == null)
                {
                    throw new InvalidOperationException($"Database property of '{dbContextTypeName}' returned null.");
                }

                Type facadeExtensionsType = typeof(RelationalDatabaseFacadeExtensions);
                MethodInfo? migrateMethod = facadeExtensionsType.GetMethod(
                    "Migrate",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    [ typeof(DatabaseFacade) ],
                    null
                );

                if (migrateMethod == null)
                {
                    throw new InvalidOperationException("Migrate method not found in RelationalDatabaseFacadeExtensions.");
                }

                migrateMethod.Invoke(null, [ databaseFacade ]);
                Console.WriteLine($"Database migration completed for '{dbContextTypeName}'.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to migrate database for '{dbContextTypeName}': {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
}
