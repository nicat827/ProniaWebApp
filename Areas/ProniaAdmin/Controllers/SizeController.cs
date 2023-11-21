using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes
                .Include(s => s.ProductSizes)
                .ToListAsync();


            return View(sizes);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = await _context.Sizes.AnyAsync(c => c.Type == size.Type.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Type", "Size with this type already exist!");
                return View();
            }

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            TempData["success"] = "Size succesfully created!";
            return RedirectToAction(nameof(Index));
        }
    }
}
