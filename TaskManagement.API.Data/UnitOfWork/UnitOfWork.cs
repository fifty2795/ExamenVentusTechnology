using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Data.Repositorio;
using TaskManagement.API.Shared.Interfaces;
using TaskManagement.API.Shared.Log;

namespace TaskManagement.API.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagementDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly ILogService _logService;
            
        public IRepositorioLogin RepositorioLogin { get; private set; }      
        public IRepositorioReporte RepositorioReporte { get; private set; }      

        public UnitOfWork(TaskManagementDbContext context, ILogService logService)
        {
            _dbContext = context;
            _logService = logService;

            RepositorioLogin = new Repositorio_Login(_dbContext, _logService);
            RepositorioReporte = new Repositorio_Reporte(_dbContext, _logService);            
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_dbContext);
            }

            return (IRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
