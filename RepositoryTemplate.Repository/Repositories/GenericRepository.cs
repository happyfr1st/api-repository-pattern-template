using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryTemplate.Repository.Context;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Repository.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    public readonly ILogger _logger;
    protected AppDbContext _context;
    internal DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<IEnumerable<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetById(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<bool> Add(T entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> Update(T entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public virtual async Task<bool> Delete(Guid id)
    { 
        var entity = await GetById(id);
        
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        
        return true;
    }
}