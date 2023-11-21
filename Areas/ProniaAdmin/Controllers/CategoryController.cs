using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

           
            return View(categories);
        }

        public IActionResult Create() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) {
                return View();
            }

            bool isExist = await _context.Categories.AnyAsync(c => c.Name == category.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Category with this name already exist!");
                return View(category);
            }

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            TempData["success"] = "Category succesfully created!";
            return RedirectToAction(nameof(Index));
        }
    }
}
