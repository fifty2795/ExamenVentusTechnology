using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ILoginService
    {
        /// <summary>
        /// Metodo para validar credenciales desde el Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseViewModel<UserDto>> Login(LoginDto request);
    }
}
