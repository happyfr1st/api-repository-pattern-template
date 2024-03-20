using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryTemplate.Data.Entities;
using RepositoryTemplate.Repository.Context;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Repository.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context, ILogger logger) : base(context, logger)
    { 
    }
    
    public async Task<RefreshToken?> GetByRefreshToken(string refreshToken)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Token == refreshToken);
    }
}