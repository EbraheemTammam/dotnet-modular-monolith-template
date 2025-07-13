using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Auth.Models;

namespace Auth.Data.Configuration;

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
