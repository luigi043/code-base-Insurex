using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InsureX.ModernAPI.Data;
using InsureX.ModernAPI.Models;

namespace InsureX.IntegrationTests
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext using in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TProgram>>>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data
                        SeedTestData(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });

            builder.UseEnvironment("Testing");
        }

        private void SeedTestData(ApplicationDbContext db)
        {
            // Add test user with CORRECT credentials (admin@insurex.com / password)
            if (!db.Users.Any())
            {
                // This is the BCrypt hash for "password"
                string passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
                
                db.Users.Add(new User
                {
                    Name = "Admin User",
                    Email = "admin@insurex.com",  // Match the expected email
                    PasswordHash = passwordHash,   // Properly hashed password
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                // Add a test user as well
                db.Users.Add(new User
                {
                    Name = "Test User",
                    Email = "test@insurex.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"),
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Add test policy
            if (!db.Policies.Any())
            {
                db.Policies.Add(new Policy
                {
                    PolicyNumber = "TEST-POL-001",
                    PolicyHolder = "Test Customer",
                    Email = "customer@test.com",
                    Phone = "123-456-7890",
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddYears(1),
                    Status = "Active",
                    Premium = 1000.00M,
                    PolicyType = "Comprehensive",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });

                db.Policies.Add(new Policy
                {
                    PolicyNumber = "TEST-POL-002",
                    PolicyHolder = "Another Customer",
                    Email = "another@test.com",
                    Phone = "098-765-4321",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1),
                    Status = "Active",
                    Premium = 2500.00M,
                    PolicyType = "Business",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            db.SaveChanges();
        }
    }
}