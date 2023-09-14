using DataPublicApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPublicApi
{
    public class PublicApiContext : DbContext
    {
        public PublicApiContext(DbContextOptions<PublicApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("user");
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public DbSet<PublicApiSettings> PublicApiSettings { get; set; }
        public DbSet<FavoriteCurrency> FavoritesCurrencies { get; set; }
    }
}
