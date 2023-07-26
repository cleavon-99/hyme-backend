using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyme.Infrastructure.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasConversion(id => id.Value, value => new ProjectId(value));
            builder.Property(p => p.Logo).HasMaxLength(50);
            builder.Property(p => p.Title).HasMaxLength(100);
            builder.Property(p => p.Banner).HasMaxLength(50);
            builder.Property(p => p.Trailer).HasMaxLength(50);
            builder.Property(p => p.ShortDescription).HasMaxLength(1000);
            builder.Property(p => p.ProjectDescription).HasMaxLength(5000);
            builder.Property(p => p.OwnerId).HasConversion(id => id.Value, value => new UserId(value));
        }
    }
}
