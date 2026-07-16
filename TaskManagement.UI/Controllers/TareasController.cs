using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.UI.Controllers
{
    public class TareasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crear()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            ViewBag.IdTarea = id;

            return View();
        }
    }
}
