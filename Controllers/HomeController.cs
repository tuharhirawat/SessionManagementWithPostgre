using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleSessionManagementWithoutDB.Models;

namespace SimpleSessionManagementWithoutDB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Public view
        }

        public IActionResult Restricted()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) || IsSessionExpired())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        private bool IsSessionExpired()
        {
            string? loginTimeStr = HttpContext.Session.GetString("LoginTime");
            if (string.IsNullOrEmpty(loginTimeStr)) return true;

            DateTime loginTime = DateTime.Parse(loginTimeStr);
            TimeSpan duration = DateTime.UtcNow - loginTime;

          
            return duration > TimeSpan.FromMinutes(1);
        }

    }
}
