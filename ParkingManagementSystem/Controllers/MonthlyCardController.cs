using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ParkingManagementSystem.Controllers
{
    public class MonthlyCardController : Controller
    {
        private readonly ParkingDbContext _context;

        public MonthlyCardController(ParkingDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. TRANG DANH SÁCH THẺ THÁNG (INDEX)
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách thẻ tháng, bao gồm thông tin Thẻ RFID, Xe và Loại xe
            var danhSach = await _context.MonthlyCards
                .Include(m => m.RFIDCard)
                .Include(m => m.Vehicle)
                    .ThenInclude(v => v.VehicleType)
                .ToListAsync();

            return View(danhSach);
        }

        // ==========================================================
        // 2. GIAO DIỆN ĐĂNG KÝ MỚI (GET)
        // ==========================================================
        public IActionResult Create()
        {
            // Nạp danh sách Loại xe (Ô tô, Xe máy...) vào ViewBag để hiện lên Dropdown
            ViewBag.VehicleTypeId = new SelectList(_context.VehicleTypes, "TypeId", "TypeName");
            return View();
        }

        // ==========================================================
        // 3. XỬ LÝ ĐĂNG KÝ (POST)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create(MonthlyCard monthlyCard, string rfidCode, string licensePlate, int vehicleTypeId)
        {
            // --- BƯỚC 1: XỬ LÝ THẺ RFID ---
            var card = await _context.RFIDCards.FirstOrDefaultAsync(c => c.RfidCode == rfidCode);
            if (card == null)
            {
                // Nếu chưa có thẻ này trong DB, tự động tạo mới
                card = new RFIDCard
                {
                    RfidCode = rfidCode,
                    UID = rfidCode, // Gán UID bằng mã thẻ để tránh lỗi NULL
                    Status = "Active"
                };
                _context.RFIDCards.Add(card);
                await _context.SaveChangesAsync();
            }
            monthlyCard.CardId = card.CardId;

            // --- BƯỚC 2: XỬ LÝ THÔNG TIN XE ---
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
            if (vehicle == null)
            {
                // Nếu xe chưa tồn tại, tạo mới kèm Loại xe đã chọn
                vehicle = new Vehicle
                {
                    LicensePlate = licensePlate,
                    TypeId = vehicleTypeId,
                    Description = "Xe đăng ký thẻ tháng"
                };
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Nếu xe đã có, cập nhật lại loại xe theo đăng ký mới nhất
                vehicle.TypeId = vehicleTypeId;
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            monthlyCard.VehicleId = vehicle.VehicleId;

            // --- BƯỚC 3: LƯU THẺ THÁNG ---
            // Loại bỏ các kiểm tra bắt buộc của các bảng liên quan để tránh lỗi Validation
            ModelState.Remove("CardId");
            ModelState.Remove("VehicleId");
            ModelState.Remove("RFIDCard");
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                _context.Add(monthlyCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, nạp lại Dropdown để quay về trang Create không bị lỗi giao diện
            ViewBag.VehicleTypeId = new SelectList(_context.VehicleTypes, "TypeId", "TypeName", vehicleTypeId);
            return View(monthlyCard);
        }

        // ==========================================================
        // 4. LOGIC NẠP TIỀN VÀ GIA HẠN (RECHARGE)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Recharge(int id, decimal amount)
        {
            var theThang = await _context.MonthlyCards.FindAsync(id);
            if (theThang != null && amount > 0)
            {
                // Cộng tiền vào số dư
                theThang.Balance += amount;

                // Tự động gia hạn thêm 30 ngày kể từ ngày cũ (hoặc từ hôm nay nếu đã hết hạn)
                if (theThang.ExpiryDate > DateTime.Now)
                {
                    theThang.ExpiryDate = theThang.ExpiryDate.AddDays(30);
                }
                else
                {
                    theThang.ExpiryDate = DateTime.Now.AddDays(30);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // 5. XÓA THẺ THÁNG (DELETE)
        // ==========================================================
        [HttpGet] // Dùng HttpGet để chạy được khi nhấn vào thẻ <a> ở giao diện
        public async Task<IActionResult> Delete(int id)
        {
            var theThang = await _context.MonthlyCards.FindAsync(id);
            if (theThang != null)
            {
                _context.MonthlyCards.Remove(theThang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}