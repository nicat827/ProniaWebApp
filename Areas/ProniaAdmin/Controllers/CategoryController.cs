using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
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
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) {
                return View();
            }

            bool isExist = await _context.Categories.AnyAsync(c => c.Name == categoryVM.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Category with this name already exist!");
                return View(categoryVM);
            }

            Category category = new Category
            {
                Name = categoryVM.Name
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            TempData["success"] = "Category succesfully created!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            UpdateCategoryVM categoryVM = new UpdateCategoryVM { Name = category.Name };
            return View(categoryVM);
        }

        [HttpPost] 
        public async Task<IActionResult> Update(int id, UpdateCategoryVM updatedCategoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedCategoryVM);
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            bool isExist = await _context.Categories.AnyAsync(c => c.Name == updatedCategoryVM.Name.Trim() && c.Id != id);
            if (isExist) {
                ModelState.AddModelError("Name", "This category already exist!");
                return View(updatedCategoryVM);
            }

            if (category.Name == updatedCategoryVM.Name) return RedirectToAction(nameof(Index));

            category.Name = updatedCategoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return NotFound();
            Category category = await _context.Categories
                .Include(c => c.Products).ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);

        }
    }
}
