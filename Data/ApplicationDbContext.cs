using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Models;

namespace PharmaChain.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Medicine>()
                .HasOne(m => m.Manufacturer)
                .WithMany()
                .HasForeignKey(m => m.ManufacturerID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.Medicine)
                .WithMany()
                .HasForeignKey(o => o.MedicineID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inventory>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inventory>()
                .HasOne(i => i.Medicine)
                .WithMany()
                .HasForeignKey(i => i.MedicineID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            builder.Entity<Medicine>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            // Configure indexes
            builder.Entity<Medicine>()
                .HasIndex(m => m.BatchNo)
                .IsUnique();

            builder.Entity<Inventory>()
                .HasIndex(i => new { i.UserID, i.MedicineID })
                .IsUnique();
        }
    }
}
