using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Accounts.Models;

namespace Accounts.Data.Configuration;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(20);
        builder.Property(u => u.LastName).HasMaxLength(20);
        builder.Property(u => u.Email).HasMaxLength(150);
        builder.Property(u => u.PhoneNumber).HasMaxLength(13);

        builder.HasOne(u => u.ProfilePicture)
               .WithOne()
               .HasForeignKey<User>(u => u.ProfilePictureId);

        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.PhoneNumber);
    }
}
