using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyme.Infrastructure.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, value => new UserId(value));
            builder.Property(u => u.WalletAddress)
                .HasConversion(walletAddress => walletAddress.Value, value => new WalletAddress(value));
            builder.Property(u => u.Name).HasMaxLength(50);
            builder.Property(u => u.WalletAddress).HasMaxLength(42);  
            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity("UsersRoles");
            builder.HasOne(u => u.Project)
                .WithOne(p => p.Owner)
                .HasForeignKey<Project>(p => p.OwnerId);
            builder.HasIndex(u => u.WalletAddress).IsUnique();
        }
    }
}
