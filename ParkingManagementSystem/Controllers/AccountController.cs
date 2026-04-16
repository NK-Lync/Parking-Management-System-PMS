using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using System.Security.Claims;

namespace ParkingManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ParkingDbContext _context;

        public AccountController(ParkingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string selectedRole)
        {
            // Kiểm tra thông tin trong Database
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName == username && u.Password == password && u.Role == selectedRole);

            if (user != null)
            {
                // Tạo "thẻ bài" định danh người dùng
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role), // Lưu quyền (Admin/Staff)
                    new Claim("Username", user.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập, mật khẩu hoặc vai trò không đúng!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}