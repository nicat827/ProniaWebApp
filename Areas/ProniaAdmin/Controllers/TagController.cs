using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{

    [Area("ProniaAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags
                .Include(t => t.ProductTags)
                .ToListAsync();


            return View(tags);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = await _context.Tags.AnyAsync(t => t.Name == tag.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Tag with this name already exist!");
                return View();
            }

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            TempData["success"] = "Tag succesfully created!";
            return RedirectToAction(nameof(Index));
        }
    }
}
