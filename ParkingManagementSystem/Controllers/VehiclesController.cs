using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;   // Đảm bảo có chữ System
using ParkingManagementSystem.Models; // Đảm bảo có chữ System

using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ParkingDbContext _context;

        public VehicleController(ParkingDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách xe (kèm theo tên loại xe)
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles.Include(v => v.VehicleType).ToListAsync();
            return View(vehicles);
        }

        // Giao diện thêm xe mới
        public IActionResult Create()
        {
            // Đổ danh sách loại xe vào Dropdown để chọn trong View
            ViewBag.TypeId = new SelectList(_context.VehicleTypes, "TypeId", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TypeId = new SelectList(_context.VehicleTypes, "TypeId", "TypeName", vehicle.TypeId);
            return View(vehicle);
        }
    }
}