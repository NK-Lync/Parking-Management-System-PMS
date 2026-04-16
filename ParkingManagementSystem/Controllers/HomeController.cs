using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;
using System.Diagnostics;

namespace ParkingManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ParkingDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ParkingDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Thống kê các con số cơ bản
            ViewBag.TotalInBãi = await _context.ParkingSessions.CountAsync(s => s.CheckOutTime == null);
            ViewBag.EmptySlots = await _context.ParkingPositions.CountAsync(p => !p.IsOccupied);
            ViewBag.TodayRevenue = await _context.ParkingSessions
                .Where(s => s.CheckOutTime.HasValue && s.CheckOutTime.Value.Date == DateTime.Today)
                .SumAsync(s => s.TotalFee) ?? 0;

            // 2. Lấy dữ liệu cho biểu đồ (Phân loại xe đang có trong bãi)
            var stats = await _context.ParkingSessions
                .Where(s => s.CheckOutTime == null && s.Vehicle != null)
                .GroupBy(s => s.Vehicle.VehicleType.TypeName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.ChartLabels = stats.Select(x => x.Name).ToList();
            ViewBag.ChartData = stats.Select(x => x.Count).ToList();

            // 3. Lấy 5 lượt vào gần nhất để hiển thị ở bảng
            var recentSessions = await _context.ParkingSessions
                .Include(s => s.Vehicle)
                .OrderByDescending(s => s.CheckInTime)
                .Take(5)
                .ToListAsync();

            return View(recentSessions); // Truyền danh sách này sang View làm Model
        }

        public async Task<IActionResult> Revenue()
        {
            var sessions = await _context.ParkingSessions
                .Include(s => s.RFIDCard)
                .Include(s => s.Vehicle).ThenInclude(v => v.VehicleType)
                .Where(s => s.CheckOutTime != null)
                .OrderByDescending(s => s.CheckOutTime)
                .ToListAsync();

            ViewBag.TodayRevenue = sessions.Where(s => s.CheckOutTime.Value.Date == DateTime.Today).Sum(s => s.TotalFee) ?? 0;
            ViewBag.MonthRevenue = sessions.Where(s => s.CheckOutTime.Value.Month == DateTime.Now.Month && s.CheckOutTime.Value.Year == DateTime.Now.Year).Sum(s => s.TotalFee) ?? 0;
            ViewBag.TotalRevenue = sessions.Sum(s => s.TotalFee) ?? 0;

            return View(sessions);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}