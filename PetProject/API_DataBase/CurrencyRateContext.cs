using API_DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_DataBase
{
    public class CurrencyRateContext : DbContext
    {
        public CurrencyRateContext(DbContextOptions options) : base(options)
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