using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;

namespace Market
{
    public class AppDbContext : DbContext
    {
        public DbSet<AutoPart> AutoParts { get; set; }
        public DbSet<ColorOption> Colors { get; set; }
        public DbSet<SizeOption> Sizes { get; set; }
        public DbSet<RelatedProduct> RelatedProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(ConfigurationManager.ConnectionStrings["AutoPartsDb"].ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация связей
            modelBuilder.Entity<AutoPart>()
                .HasMany(p => p.Colors)
                .WithOne()
                .HasForeignKey(c => c.AutoPartId);

            modelBuilder.Entity<AutoPart>()
                .HasMany(p => p.Sizes)
                .WithOne()
                .HasForeignKey(s => s.AutoPartId);

            modelBuilder.Entity<AutoPart>()
                .HasMany(p => p.RelatedProducts)
                .WithOne()
                .HasForeignKey(r => r.AutoPartId);
        }
    }

    public class ColorOption
    {
        public int Id { get; set; }
        public string AutoPartId { get; set; }
        public string Color { get; set; }
    }

    public class SizeOption
    {
        public int Id { get; set; }
        public string AutoPartId { get; set; }
        public string Size { get; set; }
    }

    public class RelatedProduct
    {
        public int Id { get; set; }
        public string AutoPartId { get; set; }
        public string RelatedPartId { get; set; }
    }
}
}
