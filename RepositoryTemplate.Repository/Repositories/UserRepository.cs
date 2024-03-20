using Microsoft.Extensions.Logging;
using RepositoryTemplate.Data.Entities;
using RepositoryTemplate.Repository.Context;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Repository.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }
}