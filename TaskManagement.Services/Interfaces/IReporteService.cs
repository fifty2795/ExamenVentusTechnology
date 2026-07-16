using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Shared.Dto;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IReporteService
    {
        /// <summary>
        /// Obtiene el resultado del reporte de las Tareas Pendientes
        /// </summary>
        /// <returns></returns>
        Task<List<ReporteTareasPendientesDto>> ObtenerTareasPendientesAsync();
    }
}
