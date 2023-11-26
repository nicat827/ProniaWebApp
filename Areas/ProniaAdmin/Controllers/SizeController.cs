using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
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
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = await _context.Sizes.AnyAsync(c => c.Type == sizeVM.Type.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Type", "Size with this type already exist!");
                return View();
            }
            Size size = new Size { Type = sizeVM.Type };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            TempData["success"] = "Size succesfully created!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id<= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();
            UpdateSizeVM sizeVM = new UpdateSizeVM { Type = size.Type };
            return View(sizeVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View(sizeVM);
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();

            bool isExist = await _context.Sizes.AnyAsync(s => s.Id != id && s.Type == sizeVM.Type);
            if (isExist)
            {
                ModelState.AddModelError("Type", "This size already exist!");
                return View(sizeVM);
            }

            size.Type = sizeVM.Type.Trim();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));




        }

        public async Task<IActionResult> Delete(int id)
        {

            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (size == null) return NotFound();
            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes
                .Include(s => s.ProductSizes).ThenInclude(ps => ps.Product).ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();

            return View(size);
        }

    }
}
