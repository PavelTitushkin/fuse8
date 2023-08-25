using DataStore.PublicApiDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataStore.PublicApiDb.EntitiesConfig
{
    public class FavoriteCurrencyConfiguration : IEntityTypeConfiguration<FavoriteCurrency>
    {
        public void Configure(EntityTypeBuilder<FavoriteCurrency> builder)
        {
            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasIndex(p=> new {p.Currency, p.BaseCurrency}).IsUnique();
        }
    }
}
