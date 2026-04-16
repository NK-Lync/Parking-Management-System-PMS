using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Controllers
{
    public class VehicleTypesController : Controller
    {
        private readonly ParkingDbContext _context;

        public VehicleTypesController(ParkingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleTypes.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeId,TypeName,PricePerHour")] VehicleType vehicleType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var vehicleType = await _context.VehicleTypes.FindAsync(id);
            if (vehicleType == null) return NotFound();
            return View(vehicleType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // SỬA TẠI ĐÂY: Thêm [Bind] để chỉ nhận 3 trường này, giúp nút Lưu hoạt động
        public async Task<IActionResult> Edit(int id, [Bind("TypeId,TypeName,PricePerHour")] VehicleType vehicleType)
        {
            if (id != vehicleType.TypeId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(vehicleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleType);
        }
    }
}