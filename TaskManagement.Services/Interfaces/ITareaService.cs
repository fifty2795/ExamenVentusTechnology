using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Helpers;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ITareaService
    {
        /// <summary>
        /// Metodo para filtrar las Tareas
        /// </summary>
        /// <param name="filtro">Filtros que se aplicaran a la consulta</param>
        /// <returns></returns>
        Task<RespuestaPaginadaDto<TareaDto>> ObtenerAsync(FiltroTareaDto filtro);

        /// <summary>
        /// Obtiene una Tarea en base a un ID
        /// </summary>
        /// <param name="id">Identificador de la Tarea</param>
        /// <returns></returns>
        Task<TareaDto?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Crea una nueva Tarea
        /// </summary>
        /// <param name="request">Contiene la data de la Tarea a crear</param>
        /// <returns></returns>
        Task<TareaDto> CrearAsync(CrearTareaDto request);

        /// <summary>
        /// Metodo para Actualizar una Tarea
        /// </summary>
        /// <param name="id">Identificador de la Tarea</param>
        /// <param name="request">Contiene los valores de los atributos a Modificar</param>
        /// <returns></returns>
        Task<bool> ActualizarAsync(int id, ActualizarTareaDto request);


        /// <summary>
        /// Metodo para Eliminar una Tarea
        /// </summary>
        /// <param name="id">Identificador de la Tarea</param>
        /// <returns></returns>
        Task<bool> EliminarAsync(int id);
    }
}
