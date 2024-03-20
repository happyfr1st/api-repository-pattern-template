using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryTemplate.Repository.Context;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Repository.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    public readonly ILogger _logger;
    internal AppDbContext _context;
    internal DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> Get(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task Add(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async void Delete(int id)
    { 
        var entity = await Get(id);
        
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}