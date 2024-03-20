using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepositoryTemplate.Data.Entities;

namespace RepositoryTemplate.Repository.Context;

public class AppDbContext : IdentityDbContext<User>
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            },
            new IdentityRole()
            {
                Id = "a9e890ff-2c71-4863-bc45-f65a82eca6db",
                Name = "User",
                NormalizedName = "User".ToUpper()
            }
        );
    }
}