using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using Base.Interfaces;
using Users.Models;
using Users.Data;
using Users.Commands;
using Users.Interfaces;
using Users.Services;
using Microsoft.AspNetCore.Http;

namespace Users.Utilities;

public class UsersModuleRegistrar : IModuleRegistrar
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<UsersDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<UsersDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<HttpContextAccessor>();

        services.AddScoped<CreateSuperUserCommand>();

        services.AddScoped<VerificationRepository>();

        services.AddScoped<IUserService, UserService>();
    }
}
