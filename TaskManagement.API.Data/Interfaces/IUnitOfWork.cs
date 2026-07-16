using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;

        IRepositorioLogin RepositorioLogin { get; }
        
        IRepositorioReporte RepositorioReporte { get; }

        Task<int> SaveChangesAsync();
    }
}
