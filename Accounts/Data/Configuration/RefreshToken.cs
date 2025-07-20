using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Accounts.Models;

namespace Accounts.Data.Configuration;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasOne(u => u.User)
               .WithOne()
               .HasForeignKey<RefreshToken>(u => u.UserId);

        builder.HasIndex(u => u.UserId);
        builder.HasIndex(u => u.Token);
    }
}
