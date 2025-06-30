using Microsoft.AspNetCore.Mvc;
using SimpleSessionManagementWithoutDB.Models;

namespace SimpleSessionManagementWithoutDB.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private const string SessionKeyName = "UserEmail";

        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                return RedirectToAction("Restricted", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                return RedirectToAction("Restricted", "Home");
            }

            if (model.Email == "test" && model.Password == "test")
            {
                HttpContext.Session.SetString(SessionKeyName, model.Email);
                HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString());

                return RedirectToAction("Restricted", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
