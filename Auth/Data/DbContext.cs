using Microsoft.EntityFrameworkCore;

using Users.Models;
using Auth.Models;

namespace Auth.Data;

public sealed class AuthDbContext : DbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        modelBuilder.Ignore<User>();
    }
}
