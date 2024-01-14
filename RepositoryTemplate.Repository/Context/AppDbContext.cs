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
    }
}