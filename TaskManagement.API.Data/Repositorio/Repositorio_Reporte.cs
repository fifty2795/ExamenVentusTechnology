using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Interfaces;

namespace TaskManagement.API.Data.Repositorio
{
    public class Repositorio_Reporte : IRepositorioReporte
    {
        private readonly TaskManagementDbContext _dbContext;
        private readonly ILogService _logService;

        public Repositorio_Reporte(TaskManagementDbContext dbContext, ILogService logService)
        {
            _dbContext = dbContext;
            _logService = logService;
        }

        public async Task<List<ReporteTareasPendientesDto>> ObtenerTareasPendientesAsync()
        {
            try
            {
                return await _dbContext.Database
                    .SqlQuery<ReporteTareasPendientesDto>(
                        $"EXEC dbo.sp_GetPendingTasks")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logService.LogError("Ocurrió un error al obtener el reporte de tareas pendientes.", ex);
                throw;
            }
        }
    }
}
