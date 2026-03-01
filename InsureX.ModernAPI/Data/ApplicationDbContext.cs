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
        public DbSet<InsuranceClaim> Claims { get; set; }  // Changed from Claim to InsuranceClaim
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure global query filters for soft delete
            modelBuilder.Entity<Policy>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Asset>().HasQueryFilter(a => !a.IsDeleted);
            
            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasDefaultValue("User");
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Policy
            modelBuilder.Entity<Policy>(entity =>
            {
                entity.HasIndex(p => p.PolicyNumber).IsUnique();
                entity.Property(p => p.Status).HasDefaultValue("Active");
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                // Relationships
                entity.HasMany(p => p.Assets)
                    .WithOne(a => a.Policy)
                    .HasForeignKey(a => a.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(p => p.Transactions)
                    .WithOne(t => t.Policy)
                    .HasForeignKey(t => t.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(p => p.Claims)
                    .WithOne(c => c.Policy)
                    .HasForeignKey(c => c.PolicyId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Asset
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.Property(a => a.Status).HasDefaultValue("Active");
                entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(a => a.JsonData).HasColumnType("nvarchar(max)");
            });

            // Configure InsuranceClaim - FIXED: Changed from Claim to InsuranceClaim
            modelBuilder.Entity<InsuranceClaim>(entity =>
            {
                entity.HasIndex(c => c.ClaimNumber).IsUnique();
                entity.Property(c => c.Status).HasDefaultValue("Pending");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                // Relationship with Policy - Use NO ACTION to avoid cascade conflict
                entity.HasOne(c => c.Policy)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(c => c.PolicyId)
                    .OnDelete(DeleteBehavior.NoAction);
                
                // Relationship with Asset - Use SET NULL
                entity.HasOne(c => c.Asset)
                    .WithMany()
                    .HasForeignKey(c => c.AssetId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => t.TransactionNumber).IsUnique();
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(t => t.Policy)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(t => t.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
