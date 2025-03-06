using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompanyManagement.Data.Data;

namespace CompanyManagement.Data.Repositories
{
  public class Repository<T> : IRepository<T> where T : class
  {
      private readonly CompanyDbContext _context;
      private readonly DbSet<T> _dbSet;

      public Repository(CompanyDbContext context)
      {
          _context = context;
          _dbSet = context.Set<T>();
      }

      public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

      // public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
      public async Task<T> GetByIdAsync(int id)
      {
        var entity = await _dbSet.FindAsync(id);
          if (entity == null)
          {
              throw new KeyNotFoundException($"Entity with ID {id} was not found.");
          }
        return entity;
      }


      public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

      public void Update(T entity) => _dbSet.Update(entity);

      public void Delete(T entity) => _dbSet.Remove(entity);

      public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
  }


} 