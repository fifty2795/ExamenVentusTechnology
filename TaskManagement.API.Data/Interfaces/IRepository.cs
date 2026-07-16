using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        Task<T> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
