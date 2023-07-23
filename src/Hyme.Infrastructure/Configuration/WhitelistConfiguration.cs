using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyme.Infrastructure.Configuration
{
    public class WhiteListConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        public void Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.HasKey(w => w.WalletAddress);
            builder.Property(w => w.WalletAddress)
                .HasConversion(walletAddress => walletAddress.Value, value => new WalletAddress(value));
        }
    }
}
