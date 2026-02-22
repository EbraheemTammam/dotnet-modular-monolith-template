using System.CommandLine;
using DotNetEnv;

using Shared.Commands;
using Accounts.Commands;
using EntryPoint.Utilities;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

string[] modules = ["Shared", "Accounts"];

builder.Services.RegisterModules(modules);
builder.Services.Configure();

var app = builder.Build();

if (args.Length > 0)
{
    using var scope = app.Services.CreateScope();
    var rootCommand = new RootCommand("ASP.NET Core CLI commands");

    if (args[0].ToLower() == "createsuperuser")
    {
        var command = scope.ServiceProvider.GetRequiredService<CreateSuperUserCommand>();
        rootCommand.Subcommands.Add(command.CreateCommand());
    }
    else if (args[0].ToLower() == "startapp")
    {
        var command = scope.ServiceProvider.GetRequiredService<StartAppCommand>();
        rootCommand.Subcommands.Add(command.CreateCommand());
    }
    else if (args[0].ToLower() == "makemigrations")
    {
        if (args.Length < 2) args = args.Concat(modules[1..]).ToArray();
        if (!modules.Contains(args[1]))
        {
            Console.WriteLine($"Module '{args[1]}' not found.");
            return;
        }
        var command = scope.ServiceProvider.GetRequiredService<MakeMigrationsCommand>();
        rootCommand.Subcommands.Add(command.CreateCommand());
    }
    await rootCommand.Parse(args).InvokeAsync();
}
else
{
    app.Configure();
    app.RegisterModules(modules[1..]);
    await app.RunAsync();
}
