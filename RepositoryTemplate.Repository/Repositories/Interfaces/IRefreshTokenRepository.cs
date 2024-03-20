using RepositoryTemplate.Data.Entities;

namespace RepositoryTemplate.Repository.Repositories.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByRefreshToken(string refreshToken);
}