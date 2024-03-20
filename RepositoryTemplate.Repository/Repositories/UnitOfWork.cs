using Microsoft.Extensions.Logging;
using RepositoryTemplate.Repository.Context;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Repository.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable 
{
    private readonly AppDbContext _context;
    
    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    
    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        var logger = loggerFactory.CreateLogger("logs");
        
        Users = new UserRepository(_context, logger);
        RefreshTokens = new RefreshTokenRepository(_context, logger);
    }

    public async Task<bool> CompleteAsync()
    {
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}