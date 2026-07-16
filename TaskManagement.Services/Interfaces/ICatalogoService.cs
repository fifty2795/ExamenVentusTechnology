using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Shared.Dto;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ICatalogoService
    {
        /// <summary>
        /// Obtiene el catalogo de Prioridades
        /// </summary>
        /// <returns></returns>
        Task<List<PrioridadDto>> ObtenerPrioridadesAsync();

        /// <summary>
        /// Obtiene el catalogo de Estatus
        /// </summary>
        /// <returns></returns>
        Task<List<EstatusDto>> ObtenerEstatusAsync();

        /// <summary>
        /// Obtiene la lista de Usuarios
        /// </summary>
        /// <returns></returns>
        Task<List<UsuarioCatalogoDto>> ObtenerUsuariosAsync();
    }
}
