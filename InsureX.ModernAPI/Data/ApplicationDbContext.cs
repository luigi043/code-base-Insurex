using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Models;

namespace InsureX.ModernAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasDefaultValue("User");
        });

        // Policy configuration
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasIndex(e => e.PolicyNumber).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.Property(e => e.Status).HasDefaultValue("Active");
        });

        // Asset configuration
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.Property(e => e.JsonData).HasColumnType("nvarchar(max)");
        });

        // Claim configuration
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasIndex(e => e.ClaimNumber).IsUnique();
            
            entity.HasOne(c => c.Asset)
                .WithMany()
                .HasForeignKey(c => c.AssetId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasIndex(e => e.TransactionNumber).IsUnique();
        });
    }
}
