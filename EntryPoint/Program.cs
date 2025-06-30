using System.CommandLine;
using DotNetEnv;

using Auth.Commands;
using EntryPoint.Utilities;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

string[] modules = ["Base", "Auth"];

builder.Services.RegisterModules(builder.Configuration, modules);
builder.Services.Configure(builder.Configuration);
builder.Services.AddScoped<CreateSuperUserCommand>();

var app = builder.Build();

if (args.Length > 0 && args[0].ToLower() == "createsuperuser")
{
    using var scope = app.Services.CreateScope();
    var command = scope.ServiceProvider.GetRequiredService<CreateSuperUserCommand>();
    var rootCommand = new RootCommand("ASP.NET Core CLI commands");
    rootCommand.AddCommand(command.CreateCommand());
    await rootCommand.InvokeAsync(args);
}
else
{
    app.Configure();
    app.RegisterModules(modules);
    await app.RunAsync();
}
