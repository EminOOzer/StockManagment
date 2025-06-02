using Microsoft.EntityFrameworkCore;
using sttok.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace sttok.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DepoStok> DepoStok { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StokHareket> StokHareketler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.UrunEn)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.UrunBoy)
                .HasPrecision(10, 2);
        }

        // Buraya DbSet'lerinizi ekleyebilirsiniz
        // Örnek: public DbSet<YourModel> YourModels { get; set; }
    }
} 