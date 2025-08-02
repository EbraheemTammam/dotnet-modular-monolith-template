using System.CommandLine;
using System.Diagnostics;

namespace Base.Commands;

public class StartAppCommand
{
    private readonly string _contextContent;
    private readonly string _moduleRegistrarContent;

    public StartAppCommand()
    {
        _contextContent = @"using Microsoft.EntityFrameworkCore;

namespace {0}.Data;

public sealed class {0}DbContext : DbContext
{
    public {0}DbContext(DbContextOptions<{0}DbContext> options)
    : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof({0}DbContext).Assembly);
    }
}";

        _moduleRegistrarContent = @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Base.Interfaces;
using {0}.Data;

namespace {0}.Utilities;

internal class {0}ModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services)
    {
        services.AddDbContextPool<{0}DbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable(""POSTGRES_CONNECTION_STRING"")));
    }
}";
    }

    public Command CreateCommand()
    {
        var appNameArg = new Argument<string>(
            name: "AppName",
            description: "App name"
        );

        var command = new Command("startapp", "Generates a new module") { appNameArg };
        string basePath = Directory.GetCurrentDirectory().Replace("/EntryPoint", string.Empty);

        command.SetHandler((appName) =>
        {
            appName = $"{appName.Substring(0, 1).ToUpper()}{appName.Substring(1).ToLower()}";
            appName = appName.Replace("-", string.Empty);
            appName = appName.Replace(" ", "_");

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new classlib -n {appName}",
                WorkingDirectory = basePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
            }

            processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"add {appName} reference Base",
                WorkingDirectory = basePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
            }

            processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"add EntryPoint reference {appName}",
                WorkingDirectory = basePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
            }

            processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"sln add {appName}",
                WorkingDirectory = basePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
            }

            File.Delete(Path.Combine(basePath, appName, $"Class1.cs"));

            string dataDir = Path.Combine(basePath, appName, "Data");
            Directory.CreateDirectory(dataDir);
            Directory.CreateDirectory(Path.Combine(dataDir, "Configuration"));

            string contextFilePath = Path.Combine(dataDir, $"DbContext.cs");
            File.WriteAllText(contextFilePath, _contextContent.Replace("{0}", appName));
            Console.WriteLine($"Created file: {contextFilePath}");

            string utilitiesDir = Path.Combine(basePath, appName, "Utilities");
            Directory.CreateDirectory(utilitiesDir);

            string moduleRegistrarFilePath = Path.Combine(utilitiesDir, $"ModuleRegistrar.cs");
            File.WriteAllText(moduleRegistrarFilePath, _moduleRegistrarContent.Replace("{0}", appName));
            Console.WriteLine($"Created file: {moduleRegistrarFilePath}");

            Directory.CreateDirectory(Path.Combine(basePath, appName, "Models"));
            Directory.CreateDirectory(Path.Combine(basePath, appName, "DTOs"));
            Directory.CreateDirectory(Path.Combine(basePath, appName, "Interfaces"));
            Directory.CreateDirectory(Path.Combine(basePath, appName, "Services"));
            Directory.CreateDirectory(Path.Combine(basePath, appName, "Controllers"));
        }, appNameArg);

        return command;
    }
}
