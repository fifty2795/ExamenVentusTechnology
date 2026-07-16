using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Shared.Response;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportsController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("pending-tasks")]
        public async Task<IActionResult> ObtenerPendientes()
        {
            var resultado = await _reporteService.ObtenerTareasPendientesAsync();

            return Ok(ResponseHelper.CrearRespuestaExito(resultado, "Reporte obtenido correctamente."));
        }
    }
}
