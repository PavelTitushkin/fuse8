using DataStore.InternalApiDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataStore.InternalApiDb
{
    public class InternalApiContext : DbContext
    {
        public InternalApiContext(DbContextOptions<InternalApiContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("cur");
            modelBuilder.HasPostgresExtension("uuid-ossp");
        }

        public DbSet<CurrencyEntity> CurrencyEntity { get; set; }
        public DbSet<Currencies> CurrenciesList { get; set; }
        public DbSet<CacheTask> CacheTasks { get; set; }
    }
}