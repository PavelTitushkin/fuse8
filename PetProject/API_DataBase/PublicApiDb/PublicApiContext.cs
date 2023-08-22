using Microsoft.EntityFrameworkCore;

namespace DataStore.PublicApiDb
{
    public class PublicApiContext : DbContext
    {
        public PublicApiContext(DbContextOptions<PublicApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("user");
        }

    }
}
