using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;

namespace TaskManagement.API.Data.Repositorio
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TaskManagementDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(TaskManagementDbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity is null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el registro de tipo {typeof(T).Name} con Id {id}.");
            }

            return entity;
        }

        public async Task AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Remove(entity);
        }
    }
}
