namespace RepositoryTemplate.Repository.Repositories.Interfaces;

public interface IUnitOfWork
{
    // IEventRepository Events { get; }
    Task<bool> CompleteAsync();
}