using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors
                .Include(c => c.ProductColors)
                .ToListAsync();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid) return View();
            bool isExist = await _context.Colors.AnyAsync(c => c.Name == color.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This color already exist!");
                return View();
            }
            await _context.Colors.AddAsync(color);

            await _context.SaveChangesAsync();
            TempData["success"] = "Color succesfully created!";
            return RedirectToAction(nameof(Index));
        }
    }
}
