namespace RepositoryTemplate.Repository.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> Get(int id);
    Task Add(T entity);
    void Update(T entity);
    void Delete(int id);
}