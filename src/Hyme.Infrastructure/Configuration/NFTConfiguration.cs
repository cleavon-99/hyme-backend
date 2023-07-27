using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyme.Infrastructure.Configuration
{
    public class NFTConfiguration : IEntityTypeConfiguration<NFT>
    {
        public void Configure(EntityTypeBuilder<NFT> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .HasConversion(id => id.Value, value => new NFTId(value));
            builder.Property(n => n.Title).HasMaxLength(200);
            builder.Property(n => n.Description).HasMaxLength(500);
            builder.Property(n => n.Image).HasMaxLength(50);
            builder.Property(p => p.ProjectId)
                .HasConversion(id => id.Value, value => new ProjectId(value));
        }
    }
}
