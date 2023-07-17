using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Crypto.Tls;

namespace Hyme.Infrastructure.Configuration
{
    public class UserprofileConfiguration : IEntityTypeConfiguration<User>
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
            builder.HasIndex(u => u.WalletAddress);
        }
    }
}
