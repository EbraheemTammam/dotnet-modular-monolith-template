using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Base.Models;

namespace Base.Data.Configuration;

internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(d => d.FileName).HasMaxLength(100);
        builder.Property(d => d.SaveTo).HasMaxLength(100);
        builder.Property(d => d.Domain).HasMaxLength(100);

        builder.HasIndex(d => d.FileName);
    }
}
