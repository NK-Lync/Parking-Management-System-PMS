using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Controllers
{
    public class RFIDCardController : Controller
    {
        private readonly ParkingDbContext _context;

        public RFIDCardController(ParkingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.RFIDCards.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(RFIDCard card)
        {
            if (ModelState.IsValid)
            {
                card.Status = "Ready"; // Mặc định thẻ mới là sẵn sàng
                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }
    }
}