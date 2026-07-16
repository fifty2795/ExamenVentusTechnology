using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Response;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITareaService _tareaService;

        public TasksController(ITareaService tareaService)
        {
            _tareaService = tareaService;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener([FromQuery] FiltroTareaDto filtro)
        {
            if (filtro.Page < 1)
            {
                return BadRequest(
                    ResponseHelper.CrearRespuestaError<object>(
                        "La página debe ser mayor a cero.",
                        StatusCodes.Status400BadRequest));
            }

            if (filtro.PageSize < 1 || filtro.PageSize > 100)
            {
                return BadRequest(
                    ResponseHelper.CrearRespuestaError<object>(
                        "El tamaño de página debe estar entre 1 y 100.",
                        StatusCodes.Status400BadRequest));
            }

            var resultado = await _tareaService.ObtenerAsync(filtro);

            return Ok(
                ResponseHelper.CrearRespuestaExito(
                    resultado,
                    "Tareas obtenidas correctamente."));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var tarea = await _tareaService.ObtenerPorIdAsync(id);

            if (tarea is null)
            {
                return NotFound(
                    ResponseHelper.CrearRespuestaError<TareaDto>(
                        "La tarea no fue encontrada.",
                        StatusCodes.Status404NotFound));
            }

            return Ok(
                ResponseHelper.CrearRespuestaExito(
                    tarea,
                    "Tarea obtenida correctamente."));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearTareaDto request)
        {
            try
            {
                var tarea = await _tareaService.CrearAsync(request);

                var response = ResponseHelper.CrearRespuestaExito(
                    tarea,
                    "Tarea creada correctamente.");

                response.Code = StatusCodes.Status201Created;

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = tarea.IdTarea },
                    response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    ResponseHelper.CrearRespuestaError<TareaDto>(
                        ex.Message,
                        StatusCodes.Status400BadRequest));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTareaDto request)
        {
            try
            {
                var actualizado = await _tareaService.ActualizarAsync(id, request);

                if (!actualizado)
                {
                    return NotFound(
                        ResponseHelper.CrearRespuestaError<object>(
                            "La tarea no fue encontrada.",
                            StatusCodes.Status404NotFound));
                }

                return Ok(
                    ResponseHelper.CrearRespuestaExito<object?>(
                        null,
                        "Tarea actualizada correctamente."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    ResponseHelper.CrearRespuestaError<object>(
                        ex.Message,
                        StatusCodes.Status400BadRequest));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _tareaService.EliminarAsync(id);

            if (!eliminado)
            {
                return NotFound(
                    ResponseHelper.CrearRespuestaError<object>(
                        "La tarea no fue encontrada.",
                        StatusCodes.Status404NotFound));
            }

            return Ok(
                ResponseHelper.CrearRespuestaExito<object?>(
                    null,
                    "Tarea eliminada correctamente."));
        }
    }
}
