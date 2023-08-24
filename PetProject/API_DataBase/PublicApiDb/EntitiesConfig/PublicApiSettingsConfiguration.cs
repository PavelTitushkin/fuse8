using DataStore.PublicApiDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataStore.PublicApiDb.EntitiesConfig
{
    public class PublicApiSettingsConfiguration : IEntityTypeConfiguration<PublicApiSettings>
    {
        public void Configure(EntityTypeBuilder<PublicApiSettings> builder)
        {
            builder.ToTable(tableBuilder =>
                    tableBuilder.HasCheckConstraint(
                        name: "rounding_cannot_be_less_than_zero",
                        sql: "currency_round_count >= 0"));
        }
    }
}
