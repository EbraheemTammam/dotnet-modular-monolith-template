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

    rootCommand.Subcommands.Add(
        scope.ServiceProvider
            .GetRequiredService<CreateSuperUserCommand>()
            .CreateCommand()
    );
    rootCommand.Subcommands.Add(
        scope.ServiceProvider
            .GetRequiredService<StartAppCommand>()
            .CreateCommand()
    );
    rootCommand.Subcommands.Add(
        scope.ServiceProvider
            .GetRequiredService<MakeMigrationsCommand>()
            .CreateCommand()
    );

    await rootCommand.Parse(args).InvokeAsync();
}
else
{
    app.Configure();
    app.RegisterModules(modules[1..]);
    await app.RunAsync();
}
