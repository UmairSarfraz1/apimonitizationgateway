using ApiMonetizationGateway.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<MonthlyUsageSummary> MonthlyUsageSummary { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.ApiKey).IsRequired().HasMaxLength(64);
                entity.Property(c => c.UserId).IsRequired();
                entity.Property(c => c.CreatedAt);
                entity.Property(c => c.IsActive);
                entity.HasIndex(c => c.ApiKey).IsUnique();

                entity.HasOne(c => c.Tier)
                      .WithMany()
                      .HasForeignKey(c => c.TierId);

                entity.HasData(
                    new Customer() { CustomerId = 1, UserId = new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75"), Email = "umair@gmail.com", Name = "Umair", TierId = 1, ApiKey = "asftdtyfqwy2332jb423ui4b3u2b324", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Customer() { CustomerId = 2, UserId = new Guid("6cd23fa8-b3b3-4926-96b3-aa111c9fee55"), Email = "ali@gmail.com", Name = "Ali", TierId = 2, ApiKey = "qwftwefqwy2332jb423ui4b3u2b334", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            });

            modelBuilder.Entity<Tier>(entity =>
            {
                entity.HasKey(c => c.TierId);
                entity.Property(c => c.Price).HasColumnType("decimal(18,2)");
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.Property(c => c.MonthlyQuota).IsRequired();
                entity.Property(c => c.RateLimit).IsRequired();


                entity.HasData(
                    new Tier() { TierId = 1, Name = "Free", Price = 0m, MonthlyQuota = 100, RateLimit = 2 },
                    new Tier() { TierId = 2, Name = "Pro", Price = 50.00m, MonthlyQuota = 100000, RateLimit = 10 });
            });

            modelBuilder.Entity<ApiUsageLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
                entity.Property(c => c.UserId).IsRequired();

                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId);

                entity.HasIndex(e => new { e.CustomerId, e.Timestamp });
                entity.HasIndex(e => e.Timestamp);
            });

            modelBuilder.Entity<MonthlyUsageSummary>(entity =>
            {
                entity.HasKey(e => e.SummaryId);
                entity.Property(e => e.MonthYear);
                entity.Property(e => e.TotalRequests).IsRequired();
                entity.Property(e => e.AmountBilled).HasColumnType("decimal(18,2)");


                entity.HasOne<Customer>()
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId);
            });
        }
    }
}