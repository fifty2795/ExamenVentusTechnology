using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Shared.Dto;
using TaskManagement.API.Shared.Interfaces;
using TaskManagement.API.Shared.Response;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet("Prioridades")]
        public async Task<IActionResult> ObtenerPrioridades()
        {
            try
            {
                var prioridades = await _catalogoService.ObtenerPrioridadesAsync();

                return Ok(ResponseHelper.CrearRespuestaExito(prioridades, "Prioridades obtenidas correctamente."));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ResponseHelper.CrearRespuestaError<List<PrioridadDto>>(
                        "Ocurrió un error al consultar las prioridades.",
                        StatusCodes.Status500InternalServerError
                    ));
            }
        }

        [HttpGet("Estatus")]
        public async Task<IActionResult> ObtenerEstatus()
        {
            try
            {
                var estatus = await _catalogoService.ObtenerEstatusAsync();

                return Ok(
                    ResponseHelper.CrearRespuestaExito(
                        estatus,
                        "Estatus obtenidos correctamente."
                    ));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ResponseHelper.CrearRespuestaError<List<EstatusDto>>(
                        "Ocurrió un error al consultar los estatus.",
                        StatusCodes.Status500InternalServerError
                    ));
            }
        }

        [HttpGet("Usuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            try
            {
                var usuarios = await _catalogoService.ObtenerUsuariosAsync();

                var response = ResponseHelper.CrearRespuestaExito(usuarios, "Usuarios obtenidos correctamente.");

                return Ok(response);
            }
            catch (Exception)
            {
                var response = ResponseHelper.CrearRespuestaError<List<UsuarioCatalogoDto>>(
                    "Ocurrió un error al consultar los usuarios.",
                        StatusCodes.Status500InternalServerError
                    );

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    response
                );
            }
        }
    }
}
