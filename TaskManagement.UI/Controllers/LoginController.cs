using Microsoft.AspNetCore.Mvc;
using TaskManagement.UI.Models;

namespace TaskManagement.UI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            if (loginDto.Email == "admin@hotmail.com" && loginDto.Password == "123")
            {
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }

            return Json(new { success = false, redirectUrl = Url.Action("Index", "Login") });
        }
    }
}
