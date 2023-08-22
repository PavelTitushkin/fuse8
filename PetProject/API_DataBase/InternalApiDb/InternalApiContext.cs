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
        }

        public DbSet<CurrencyEntity> CurrencyEntity { get; set; }
        public DbSet<Currencies> CurrenciesList { get; set; }
    }
}