using Microsoft.EntityFrameworkCore;
using TailorrNow.Models;

namespace TailorrNow.Models
{
    public class CustomTablesContext : DbContext
    {
        public CustomTablesContext(DbContextOptions<CustomTablesContext> options)
               : base(options)
        {
        }

        public DbSet<Tailor> Tailors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Commission> Commissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                
                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                
                entity.HasOne(b => b.Tailor)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(b => b.TailorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(s => s.Price)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Commission>(entity =>
            {
                entity.Property(c => c.Amount)
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}