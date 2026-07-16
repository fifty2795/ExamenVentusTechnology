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
using TaskManagement.API.Shared.Helpers;
using TaskManagement.API.Shared.Interfaces;

namespace TaskManagement.API.Services.Services
{
    public class TareaService : ITareaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;

        public TareaService(IUnitOfWork unitOfWork, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
        }

        public async Task<RespuestaPaginadaDto<TareaDto>> ObtenerAsync(FiltroTareaDto filtro)
        {
            var query = _unitOfWork.Repository<Tarea>().Query().AsNoTracking().AsQueryable();

            if (filtro.IdPrioridad.HasValue)
            {
                query = query.Where(t => t.IdPrioridad == filtro.IdPrioridad.Value);
            }

            if (filtro.IdEstatus.HasValue)
            {
                query = query.Where(t => t.IdEstatus == filtro.IdEstatus.Value);
            }

            if (filtro.IdUsuarioResponsable.HasValue)
            {
                query = query.Where(t => t.IdUsuarioResponsable == filtro.IdUsuarioResponsable.Value);
            }

            if (filtro.FechaInicial.HasValue)
            {
                query = query.Where(t => t.FechaCreacion >= filtro.FechaInicial.Value);
            }

            if (filtro.FechaFinal.HasValue)
            {
                var fechaFinal = filtro.FechaFinal.Value.Date.AddDays(1);

                query = query.Where(t => t.FechaCreacion < fechaFinal);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(t => new TareaDto
                {
                    IdTarea = t.IdTarea,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    IdPrioridad = t.IdPrioridad,
                    Prioridad = t.IdPrioridadNavigation.Nombre,
                    FechaCreacion = t.FechaCreacion,
                    FechaInicio = t.FechaInicio,
                    FechaFinalizacion = t.FechaFinalizacion,
                    FechaLimite = t.FechaLimite,
                    IdEstatus = t.IdEstatus,
                    Estatus = t.IdEstatusNavigation.Nombre,
                    IdUsuarioResponsable = t.IdUsuarioResponsable,
                    UsuarioResponsable =
                        t.IdUsuarioResponsableNavigation.NombreCompleto
                })
                .ToListAsync();

            return new RespuestaPaginadaDto<TareaDto>
            {
                Items = items,
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filtro.PageSize)
            };
        }

        public async Task<TareaDto?> ObtenerPorIdAsync(int id)
        {
            return await _unitOfWork
                .Repository<Tarea>()
                .Query()
                .AsNoTracking()
                .Where(t => t.IdTarea == id)
                .Select(t => new TareaDto
                {
                    IdTarea = t.IdTarea,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    IdPrioridad = t.IdPrioridad,
                    Prioridad = t.IdPrioridadNavigation.Nombre,
                    FechaCreacion = t.FechaCreacion,
                    FechaInicio = t.FechaInicio,
                    FechaFinalizacion = t.FechaFinalizacion,
                    FechaLimite = t.FechaLimite,
                    IdEstatus = t.IdEstatus,
                    Estatus = t.IdEstatusNavigation.Nombre,
                    IdUsuarioResponsable = t.IdUsuarioResponsable,
                    UsuarioResponsable =
                        t.IdUsuarioResponsableNavigation.NombreCompleto
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TareaDto> CrearAsync(CrearTareaDto request)
        {
            await ValidarTareaAsync(request);

            var tarea = new Tarea
            {
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                IdPrioridad = request.IdPrioridad,
                FechaCreacion = DateTime.UtcNow,
                FechaInicio = request.FechaInicio,
                FechaFinalizacion = request.FechaFinalizacion,
                FechaLimite = request.FechaLimite,
                IdEstatus = request.IdEstatus,
                IdUsuarioResponsable = request.IdUsuarioResponsable
            };

            await _unitOfWork.Repository<Tarea>().AddAsync(tarea);

            var registrosAfectados = await _unitOfWork.SaveChangesAsync();

            if (registrosAfectados <= 0)
            {
                _logService.LogError($"No fue posible crear la tarea. Tarea: { request.Titulo } ");

                throw new InvalidOperationException("No fue posible crear la tarea.");
            }

            return await ObtenerPorIdAsync(tarea.IdTarea) ?? throw new InvalidOperationException("No fue posible obtener la tarea creada.");
        }

        public async Task<bool> ActualizarAsync(int id, ActualizarTareaDto request)
        {
            var tarea = await _unitOfWork.Repository<Tarea>().GetByIdAsync(id);

            if (tarea is null) return false;

            await ValidarTareaAsync(request, id);

            tarea.Titulo = request.Titulo.Trim();
            tarea.Descripcion = request.Descripcion?.Trim();
            tarea.IdPrioridad = request.IdPrioridad;
            tarea.FechaInicio = request.FechaInicio;
            tarea.FechaFinalizacion = request.FechaFinalizacion;
            tarea.FechaLimite = request.FechaLimite;
            tarea.IdEstatus = request.IdEstatus;
            tarea.IdUsuarioResponsable = request.IdUsuarioResponsable;

            _unitOfWork.Repository<Tarea>().Update(tarea);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tarea = await _unitOfWork.Repository<Tarea>().GetByIdAsync(id);

            if (tarea is null) return false;

            _unitOfWork.Repository<Tarea>().Remove(tarea);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task ValidarTareaAsync(CrearTareaDto request, int? idTarea = null)
        {
            if (request.FechaLimite < DateOnly.FromDateTime(DateTime.Today))
            {
                _logService.LogWarning("La fecha límite no puede ser menor a la fecha actual.");

                throw new ArgumentException("La fecha límite no puede ser menor a la fecha actual.");
            }

            var tituloDuplicado = await _unitOfWork
                .Repository<Tarea>()
                .Query()
                .AsNoTracking()
                .AnyAsync(t =>
                    t.IdUsuarioResponsable ==
                        request.IdUsuarioResponsable &&
                    t.Titulo == request.Titulo.Trim() &&
                    (!idTarea.HasValue || t.IdTarea != idTarea.Value));

            if (tituloDuplicado)
            {
                _logService.LogError($"El usuario con ID: { request.IdUsuarioResponsable } ya tiene una tarea con el mismo título '{ request.Titulo }'.");

                throw new ArgumentException("El usuario ya tiene una tarea con el mismo título.");
            }

            var usuarioExiste = await _unitOfWork
                .Repository<Usuario>()
                .Query()
                .AsNoTracking()
                .AnyAsync(u =>
                    u.IdUsuario == request.IdUsuarioResponsable &&
                    u.Activo);

            if (!usuarioExiste)
            {
                _logService.LogError($"El usuario con ID: {request.IdUsuarioResponsable} no existe o está inactivo.");

                throw new ArgumentException("El usuario responsable no existe o está inactivo.");
            }

            var prioridadExiste = await _unitOfWork
                .Repository<CatPrioridade>()
                .Query()
                .AsNoTracking()
                .AnyAsync(p => p.IdPrioridad == request.IdPrioridad);

            if (!prioridadExiste)
            {
                _logService.LogError($"La prioridad seleccionada no existe. ID Prioridad: { request.IdPrioridad }");

                throw new ArgumentException("La prioridad seleccionada no existe.");
            }

            var estatusExiste = await _unitOfWork
                .Repository<CatEstatus>()
                .Query()
                .AsNoTracking()
                .AnyAsync(e => e.IdEstatus == request.IdEstatus);

            if (!estatusExiste)
            {
                _logService.LogError($"El estatus seleccionado no existe. Estatus: { request.IdEstatus }");

                throw new ArgumentException("El estatus seleccionado no existe.");
            }
        }
    }
}
