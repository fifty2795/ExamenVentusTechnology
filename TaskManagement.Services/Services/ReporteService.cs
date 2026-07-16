using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Shared.Dto;

namespace TaskManagement.API.Services.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReporteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }        

        public async Task<List<ReporteTareasPendientesDto>>ObtenerTareasPendientesAsync()
        {
           return await _unitOfWork.RepositorioReporte.ObtenerTareasPendientesAsync();
        }
    }
}
