using System.CommandLine;
using System.Diagnostics;

namespace Base.Commands;

public class MakeMigrationsCommand
{
    public MakeMigrationsCommand(){}

    public Command CreateCommand()
    {
        var appNameArg = new Argument<string>(name: "ModuleName");
        var nameFlag = new Option<string>(name: "--name");

        var command = new Command("makemigrations", "Make migration files for specified app") { appNameArg, nameFlag };

        command.SetAction(ParseResult =>
        {
            string? appName = ParseResult.GetValue(appNameArg);
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentException("App Name is required.");

            string? migrationName = ParseResult.GetValue(nameFlag);
            if (string.IsNullOrWhiteSpace(migrationName))
                migrationName = $"{Guid.NewGuid()}_{appName}";

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef migrations add {migrationName} --project ../{appName} --context {appName}DbContext --output-dir Data/Migrations",
                WorkingDirectory = Directory.GetCurrentDirectory(),
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

                Console.WriteLine("Output:");
                Console.WriteLine(output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Errors:");
                    Console.WriteLine(error);
                }
            }
        });

        return command;
    }
}
