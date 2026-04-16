using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagementSystem.Controllers
{
    public class ParkingSessionController : Controller
    {
        private readonly ParkingDbContext _context;

        public ParkingSessionController(ParkingDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. TRANG LỊCH SỬ RA VÀO (INDEX)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var parkingSessions = await _context.ParkingSessions
                .Include(p => p.RFIDCard)
                .Include(p => p.Vehicle)
                    .ThenInclude(v => v.VehicleType) // Lấy tên loại xe để hiện Ô tô/Xe máy
                .OrderByDescending(p => p.CheckInTime)
                .ToListAsync();

            return View(parkingSessions);
        }

        // ==========================================
        // 2. TRẠM KIỂM SOÁT (MONITOR / ENTRY)
        // ==========================================
        public IActionResult Monitor()
        {
            // Lấy vị trí còn trống
            ViewBag.AvailablePositions = _context.ParkingPositions
                .Where(p => !p.IsOccupied)
                .ToList();

            // Lấy 5 giao dịch gần nhất hiện lên bảng nhật ký
            ViewBag.RecentSessions = _context.ParkingSessions
                .Include(s => s.Vehicle)
                .OrderByDescending(s => s.CheckInTime)
                .Take(5)
                .ToList();

            return View();
        }

        // XỬ LÝ XE VÀO
        [HttpPost]
        public async Task<IActionResult> ProcessCheckIn(string rfidCode, string licensePlate, int positionId, string capturedImageBase64)
        {
            if (string.IsNullOrEmpty(capturedImageBase64))
            {
                TempData["Error"] = "Lỗi: Camera chưa chụp được ảnh!";
                return RedirectToAction("Monitor");
            }

            // Lưu ảnh vào thư mục wwwroot/uploads
            string fileName = $"In_{licensePlate}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            var base64Data = capturedImageBase64.Contains(",") ? capturedImageBase64.Split(',')[1] : capturedImageBase64;
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            // Tìm thẻ RFID và thông tin xe nếu đã đăng ký
            var card = await _context.RFIDCards.FirstOrDefaultAsync(c => c.RfidCode == rfidCode);
            var registeredVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

            var session = new ParkingSession
            {
                CardId = card?.CardId,
                VehicleId = registeredVehicle?.VehicleId, // Nếu là xe cư dân thì gán Id vào
                LicensePlateIn = licensePlate,
                PositionId = positionId,
                CheckInTime = DateTime.Now,
                ImageIn = "/uploads/" + fileName
            };

            // Cập nhật trạng thái vị trí đỗ
            var pos = await _context.ParkingPositions.FindAsync(positionId);
            if (pos != null) pos.IsOccupied = true;

            _context.Add(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Xe {licensePlate} đã vào bãi thành công!";
            return RedirectToAction("Monitor");
        }

        // ==========================================
        // 3. XỬ LÝ XE RA (CHECK OUT)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> ProcessCheckOut(string rfidCode)
        {
            // 1. Tìm phiên gửi xe đang ở trong bãi (CheckOutTime == null)
            var session = await _context.ParkingSessions
                .Include(s => s.RFIDCard)
                .Include(s => s.Vehicle)
                    .ThenInclude(v => v.VehicleType) // Quan trọng: Lấy giá tiền từ đây
                .FirstOrDefaultAsync(s => s.RFIDCard.RfidCode == rfidCode && s.CheckOutTime == null);

            if (session == null)
            {
                TempData["Error"] = "Thẻ này chưa quẹt vào hoặc không hợp lệ!";
                return RedirectToAction("Monitor");
            }

            session.CheckOutTime = DateTime.Now;

            // 2. KIỂM TRA THẺ THÁNG (FREE)
            var isMonthlyCard = await _context.MonthlyCards
                .AnyAsync(m => m.CardId == session.CardId && m.ExpiryDate >= DateTime.Now);

            if (isMonthlyCard)
            {
                session.TotalFee = 0;
                TempData["Note"] = "Xe đăng ký tháng - Miễn phí";
            }
            else
            {
                // 3. TÍNH TIỀN THEO GIÁ ĐÃ CẤU HÌNH TRONG ADMIN
                // Lấy giá từ bảng VehicleType, nếu không thấy thì mặc định 5.000đ
                decimal hourlyRate = session.Vehicle?.VehicleType?.PricePerHour ?? 5000;

                var duration = session.CheckOutTime.Value - session.CheckInTime.Value;
                // Làm tròn lên số giờ (ví dụ gửi 1h15p tính là 2h)
                double totalHours = Math.Ceiling(duration.TotalHours);
                if (totalHours < 1) totalHours = 1; // Tối thiểu 1 giờ

                session.TotalFee = (decimal)totalHours * hourlyRate;
            }

            // 4. Giải phóng vị trí đỗ
            var pos = await _context.ParkingPositions.FindAsync(session.PositionId);
            if (pos != null) pos.IsOccupied = false;

            await _context.SaveChangesAsync();

            // Truyền thông tin ra màn hình Monitor
            TempData["LastPlate"] = session.LicensePlateIn;
            TempData["LastFee"] = session.TotalFee?.ToString("N0") + " VNĐ";
            TempData["Success"] = "Xe đã ra bãi thành công!";

            return RedirectToAction("Monitor");
        }

        // ==========================================
        // 4. HÓA ĐƠN (RECEIPT)
        // ==========================================
        public async Task<IActionResult> Receipt(int id)
        {
            var session = await _context.ParkingSessions
                .Include(s => s.Vehicle).ThenInclude(v => v.VehicleType)
                .Include(s => s.RFIDCard)
                .FirstOrDefaultAsync(m => m.SessionId == id);

            if (session == null) return NotFound();

            return View(session);
        }
    }
}