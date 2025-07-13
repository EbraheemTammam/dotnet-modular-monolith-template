using System.CommandLine;
using DotNetEnv;
using Twilio;

using Base.Commands;
using Users.Commands;
using EntryPoint.Utilities;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

string[] modules = ["Base", "Users", "Auth"];

builder.Services.RegisterModules(modules);
builder.Services.Configure();

TwilioClient.Init(
    Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"),
    Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")
);

var app = builder.Build();

if (args.Length > 0)
{
    using var scope = app.Services.CreateScope();
    var rootCommand = new RootCommand("ASP.NET Core CLI commands");

    if (args[0].ToLower() == "createsuperuser")
    {
        var command = scope.ServiceProvider.GetRequiredService<CreateSuperUserCommand>();
        rootCommand.AddCommand(command.CreateCommand());
    }
    else if (args[0].ToLower() == "startapp")
    {
        var command = scope.ServiceProvider.GetRequiredService<StartAppCommand>();
        rootCommand.AddCommand(command.CreateCommand());
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
        rootCommand.AddCommand(command.CreateCommand());
    }
    await rootCommand.InvokeAsync(args);
}
else
{
    app.Configure();
    app.RegisterModules(modules);
    await app.RunAsync();
}
