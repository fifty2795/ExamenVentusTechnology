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
    public class Repositorio_Login : Repository<Usuario>, IRepositorioLogin
    {
        private readonly ILogService _logService;

        public Repositorio_Login(TaskManagementDbContext context, ILogService logService)
               : base(context)
        {
            _logService = logService;
        }

        public async Task<UserDto> Login(LoginDto request)
        {
            try
            {
                var usuario = await _dbContext.Usuarios
                                .Where(u =>
                                    u.CorreoElectronico == request.Email &&
                                    u.Password == request.Password &&
                                    u.Activo)
                                .Select(u => new UserDto
                                {
                                    IdUsuario = u.IdUsuario,
                                    NombreCompleto = u.NombreCompleto,
                                    CorreoElectronico = u.CorreoElectronico
                                })
                                .FirstOrDefaultAsync();
                
                return usuario;
            }
            catch (Exception ex)
            {
                _logService.LogError("Ocurrio un error en el Repositorio Usuario en el Metodo: Login()", ex);
                throw;
            }
        }
    }
}
