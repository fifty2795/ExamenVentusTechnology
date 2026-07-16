using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Interfaces;
using TaskManagement.API.Shared.Log;
using TaskManagement.API.Shared.Response;

namespace TaskManagement.API.Services.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;        
        private readonly ILogService _logService;        

        public LoginService(IUnitOfWork unitOfWork, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
        }

        public async Task<ResponseViewModel<UserDto>> Login(LoginDto request)
        {
            var usuario = await _unitOfWork.RepositorioLogin.Login(request);

            if (usuario == null)
                return ResponseHelper.CrearRespuestaError<UserDto>("Credenciales inválidas.", 401);

            _logService.LogInfo($"El usuario { request.Email } inicio sesión a las { DateTime.Now} ");

            return ResponseHelper.CrearRespuestaExito(usuario, "Usuario autenticado exitosamente.");
        }
    }
}
