using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Models;

namespace InsureX.ModernAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<InsuranceClaim> Claims { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        // New billing tables
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RecurringBilling> RecurringBillings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Decimal precision configuration
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.Property(e => e.FinanceValue).HasPrecision(18, 2);
                entity.Property(e => e.InsuredValue).HasPrecision(18, 2);
                entity.HasOne(a => a.Policy)
                    .WithMany(p => p.Assets)
                    .HasForeignKey(a => a.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InsuranceClaim>(entity =>
            {
                entity.Property(e => e.ClaimAmount).HasPrecision(18, 2);
                entity.Property(e => e.ApprovedAmount).HasPrecision(18, 2);
                entity.HasOne(c => c.Policy)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(c => c.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.Premium).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(t => t.Policy)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(t => t.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Invoice configurations
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Tax).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasOne(i => i.Policy)
                    .WithMany()
                    .HasForeignKey(i => i.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(i => i.InvoiceNumber).IsUnique();
                entity.HasIndex(i => i.Status);
                entity.HasIndex(i => i.DueDate);
            });

            // InvoiceItem configurations
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(ii => ii.Invoice)
                    .WithMany(i => i.InvoiceItems)
                    .HasForeignKey(ii => ii.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment configurations
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(p => p.Invoice)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(p => p.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(p => p.PaymentNumber).IsUnique();
                entity.HasIndex(p => p.Status);
            });

            // RecurringBilling configurations
            modelBuilder.Entity<RecurringBilling>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(r => r.Policy)
                    .WithMany()
                    .HasForeignKey(r => r.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(r => r.NextBillingDate);
                entity.HasIndex(r => r.IsActive);
            });
        }
    }
}
