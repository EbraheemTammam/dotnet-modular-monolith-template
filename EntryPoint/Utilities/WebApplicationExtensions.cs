using Microsoft.AspNetCore.Identity;

namespace EntryPoint.Utilities;

public static class WebAppExtensions
{
    public static void Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseCors("CorsPolicy");
        app.UseAuthorization();
        app.MapControllers();
        app.UseStaticFiles();
        // if(app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }
        app.Lifetime.ApplicationStarted.Register(async () => await app.LoadDefaultData());
    }

    public static void RegisterModules(this WebApplication app, string[] modules)
    {
        foreach (string module in modules)
        {
            if (module == "Base") continue;
            app.Services.MigrateDatabase(module);
        }
    }

    private static async Task LoadDefaultData(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await roleManager.CreateRolesIfNotExist(["superuser"]);
    }

    private static async Task CreateRolesIfNotExist(this RoleManager<IdentityRole<Guid>> roleManager, string[] roles)
    {
        foreach(string role in roles)
        {
            if(await roleManager.RoleExistsAsync(role)) continue;
            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            if(result.Succeeded) continue;
            foreach(var e in result.Errors) Console.WriteLine(e.Description);
        }
    }
}
