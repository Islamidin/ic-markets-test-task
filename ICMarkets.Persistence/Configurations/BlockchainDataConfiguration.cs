using ICMarkets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarkets.Persistence.Configurations;

public class BlockchainDataConfiguration : IEntityTypeConfiguration<BlockchainData>
{
    public void Configure(EntityTypeBuilder<BlockchainData> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Blockchain)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(x => x.RawJson)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.ToTable("BlockchainData");
    }
}