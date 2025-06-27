//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Identity;
//using MimeKit;
//using MimeKit.Text;
//using MailKit.Net.Smtp;
//using ZoomColorLab.Models;
//using System;
//using System.Linq;

//namespace ZoomColorLab.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        private const string GmailUser = "haripriyarangumudri@gmail.com";
//        private const string GmailAppPassword = "erktdvpzgoooflmp"; 

//        public AccountController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Login(string username, string password)
//        {
//            var user = _context.StaffCredentials.FirstOrDefault(u => u.UserName == username && u.Active == "Y");
//            if (user != null)
//            {
//                var hasher = new PasswordHasher<StaffCredentials>();
//                var result = hasher.VerifyHashedPassword(user, user.Password, password);
//                if (result == PasswordVerificationResult.Success)
//                {
//                    TempData["SuccessMessage"] = "Login successful!";
//                    return RedirectToAction("Create", "StaffReg");
//                }
//            }

//            ViewBag.Error = "Invalid username or password.";
//            return View();
//        }

//        [HttpGet]
//        public IActionResult ForgotPassword()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult ForgotPassword(string usernameOrEmail)
//        {
//            var staffUser = _context.StaffCredentials.FirstOrDefault(u => u.UserName == usernameOrEmail && u.Active == "Y");

//            if (staffUser == null)
//            {
//                var contact = _context.StaffContacts.FirstOrDefault(c => c.Email == usernameOrEmail);
//                if (contact != null)
//                {
//                    staffUser = _context.StaffCredentials.FirstOrDefault(u => u.StaffId == contact.StaffId && u.Active == "Y");
//                }
//            }

//            if (staffUser == null)
//            {
//                ViewBag.Error = "No matching user found.";
//                return View(); 
//            }

//            string recipientEmail = null;
//            if (usernameOrEmail.Contains("@"))
//            {
//                recipientEmail = usernameOrEmail;
//            }
//            else
//            {
//                var contact = _context.StaffContacts.FirstOrDefault(c => c.StaffId == staffUser.StaffId);
//                recipientEmail = contact?.Email;
//            }

//            if (string.IsNullOrEmpty(recipientEmail) || !recipientEmail.Contains("@"))
//            {
//                ViewBag.Error = "A valid email address was not found for this user.";
//                return View(); 
//            }

//            string token = Guid.NewGuid().ToString();
//            var resetToken = new PasswordResetToken
//            {
//                StaffId = staffUser.StaffId,
//                Token = token,
//                Expiration = DateTime.UtcNow.AddHours(1)
//            };
//            _context.PasswordResetTokens.Add(resetToken);
//            _context.SaveChanges();

//            string resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);


//            var email = new MimeMessage();
//            email.From.Add(MailboxAddress.Parse(GmailUser));
//            email.To.Add(MailboxAddress.Parse(recipientEmail));
//            email.Subject = "Password Reset - ZoomColorLab";
//            email.Body = new TextPart(TextFormat.Plain)
//            {
//                Text = $"Hello,\n\nClick the link to reset your password:\n{resetLink}\n\nThis link expires in 1 hour."
//            };

//            try
//            {
//                using var smtp = new SmtpClient();
//                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
//                smtp.Authenticate(GmailUser, GmailAppPassword);
//                smtp.Send(email);
//                smtp.Disconnect(true);
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = $"Failed to send email: {ex.Message}";
//                return View();
//            }

//            TempData["SuccessMessage"] = "Password reset instructions sent to your email.";
//            return RedirectToAction("Login");
//        }

//        [HttpGet]
//        public IActionResult ResetPassword(string token)
//        {
//            var tokenEntry = _context.PasswordResetTokens
//                .FirstOrDefault(t => t.Token == token && t.Expiration > DateTime.UtcNow);

//            if (tokenEntry == null)
//                return NotFound("Invalid or expired token.");

//            ViewBag.Token = token;
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult ResetPassword(string token, string newPassword)
//        {
//            var tokenEntry = _context.PasswordResetTokens
//                .FirstOrDefault(t => t.Token == token && t.Expiration > DateTime.UtcNow);

//            if (tokenEntry == null)
//                return NotFound("Invalid or expired token.");

//            var staffUser = _context.StaffCredentials.FirstOrDefault(u => u.StaffId == tokenEntry.StaffId);
//            if (staffUser == null)
//                return NotFound("User not found.");

//            var passwordHasher = new PasswordHasher<StaffCredentials>();
//            staffUser.Password = passwordHasher.HashPassword(staffUser, newPassword);

//            _context.PasswordResetTokens.Remove(tokenEntry);
//            _context.SaveChanges();

//            TempData["SuccessMessage"] = "Password reset successful!";
//            return RedirectToAction("Login");
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using ZoomColorLab.Models;
using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ZoomColorLab.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailSettings _emailSettings;

        public AccountController(ApplicationDbContext context, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailSettings = emailSettings.Value;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _context.StaffCredentials.FirstOrDefault(u => u.UserName == username && u.Active == "Y");
            if (user != null)
            {
                var hasher = new PasswordHasher<StaffCredentials>();
                var result = hasher.VerifyHashedPassword(user, user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Create", "StaffReg");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string usernameOrEmail)
        {
            var staffUser = _context.StaffCredentials.FirstOrDefault(u => u.UserName == usernameOrEmail && u.Active == "Y");

            if (staffUser == null)
            {
                var contact = _context.StaffContacts.FirstOrDefault(c => c.Email == usernameOrEmail);
                if (contact != null)
                {
                    staffUser = _context.StaffCredentials.FirstOrDefault(u => u.StaffId == contact.StaffId && u.Active == "Y");
                }
            }

            if (staffUser == null)
            {
                ViewBag.Error = "No matching user found.";
                return View();
            }

            string recipientEmail = usernameOrEmail.Contains("@")
                ? usernameOrEmail
                : _context.StaffContacts.FirstOrDefault(c => c.StaffId == staffUser.StaffId)?.Email;

            if (string.IsNullOrEmpty(recipientEmail) || !recipientEmail.Contains("@"))
            {
                ViewBag.Error = "A valid email address was not found for this user.";
                return View();
            }

            string token = Guid.NewGuid().ToString();
            var resetToken = new PasswordResetToken
            {
                StaffId = staffUser.StaffId,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            };
            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();

            string resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.GmailUser));
            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = "Password Reset - ZoomColorLab";
            email.Body = new TextPart(TextFormat.Plain)
            {
                Text = $"Hello,\n\nClick the link to reset your password:\n{resetLink}\n\nThis link expires in 1 hour."
            };

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSettings.GmailUser, _emailSettings.GmailAppPassword);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to send email: {ex.Message}";
                return View();
            }

            TempData["SuccessMessage"] = "Password reset instructions sent to your email.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var tokenEntry = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == token && t.Expiration > DateTime.UtcNow);

            if (tokenEntry == null)
                return NotFound("Invalid or expired token.");

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var tokenEntry = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == token && t.Expiration > DateTime.UtcNow);

            if (tokenEntry == null)
                return NotFound("Invalid or expired token.");

            var staffUser = _context.StaffCredentials.FirstOrDefault(u => u.StaffId == tokenEntry.StaffId);
            if (staffUser == null)
                return NotFound("User not found.");

            var passwordHasher = new PasswordHasher<StaffCredentials>();
            staffUser.Password = passwordHasher.HashPassword(staffUser, newPassword);

            _context.PasswordResetTokens.Remove(tokenEntry);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password reset successful!";
            return RedirectToAction("Login");
        }
    }
}
