using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Shared.Dto;

namespace TaskManagement.API.Data.Interfaces
{
    public interface IRepositorioLogin : IRepository<Usuario>
    {
        Task<UserDto> Login(LoginDto request);
    }
}
