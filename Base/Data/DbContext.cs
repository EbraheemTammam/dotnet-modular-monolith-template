using Microsoft.EntityFrameworkCore;

using Base.Models;

namespace Base.Data;

public class BaseDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }

    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
    }
}
