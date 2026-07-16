using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.UI.Controllers
{
    public class ReportesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
