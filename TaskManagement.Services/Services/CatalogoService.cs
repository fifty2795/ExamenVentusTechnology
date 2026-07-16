using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Shared.Dto;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PrioridadDto>> ObtenerPrioridadesAsync()
        {
            return await _unitOfWork
                .Repository<CatPrioridade>()
                .Query()
                .AsNoTracking()
                .OrderBy(p => p.IdPrioridad)
                .Select(p => new PrioridadDto
                {
                    IdPrioridad = p.IdPrioridad,
                    Nombre = p.Nombre
                })
                .ToListAsync();
        }

        public async Task<List<EstatusDto>> ObtenerEstatusAsync()
        {
            return await _unitOfWork
                .Repository<CatEstatus>()
                .Query()
                .AsNoTracking()
                .OrderBy(e => e.IdEstatus)
                .Select(e => new EstatusDto
                {
                    IdEstatus = e.IdEstatus,
                    Nombre = e.Nombre
                })
                .ToListAsync();
        }

        public async Task<List<UsuarioCatalogoDto>> ObtenerUsuariosAsync()
        {
            return await _unitOfWork
                .Repository<Usuario>()
                .Query()
                .AsNoTracking()
                .Where(u => u.Activo)
                .OrderBy(u => u.NombreCompleto)
                .Select(u => new UsuarioCatalogoDto
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = u.NombreCompleto,
                    CorreoElectronico = u.CorreoElectronico
                })
                .ToListAsync();
        }
    }
}
