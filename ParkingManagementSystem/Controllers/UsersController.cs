using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly ParkingDbContext _context;

        public UsersController(ParkingDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DANH SÁCH NHÂN VIÊN (INDEX)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // ==========================================
        // 2. THÊM MỚI NHÂN VIÊN (GET)
        // ==========================================
        public IActionResult Create() => View();

        // ==========================================
        // 3. THÊM MỚI NHÂN VIÊN (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,UserName,Password,Role,PhoneNumber,IsActive")] User user)
        {
            // KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP
            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // ==========================================
        // 4. SỬA THÔNG TIN NHÂN VIÊN (GET)
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ==========================================
        // 5. SỬA THÔNG TIN NHÂN VIÊN (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,UserName,Password,Role,PhoneNumber,IsActive")] User user)
        {
            if (id != user.UserId) return NotFound();

            // 1. KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP (TRỪ CHÍNH NGƯỜI ĐANG SỬA)
            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName && u.UserId != id))
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập này đã được người khác sử dụng.");
            }

            // Lấy dữ liệu người dùng hiện tại từ Database để lấy mật khẩu cũ
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
            if (existingUser == null) return NotFound();

            // Nếu mật khẩu gửi lên bị trống, hãy dùng lại mật khẩu cũ
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = existingUser.Password;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId)) return NotFound();
                    else throw;
                }
            }
            return View(user);
        }

        // ==========================================
        // 6. XÓA TÀI KHOẢN
        // ==========================================
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}