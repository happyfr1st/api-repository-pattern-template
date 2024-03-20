namespace RepositoryTemplate.Repository.Repositories.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    
    Task<bool> CompleteAsync();
}