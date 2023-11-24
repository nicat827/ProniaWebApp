using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using System.Drawing;

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

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Tag updatedTag)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedTag);
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();

            bool isExist = await _context.Tags.AnyAsync(t => t.Name == updatedTag.Name.Trim() && t.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This tag already exist!");
                return View(tag);
            }

            if (tag.Name == updatedTag.Name) return RedirectToAction(nameof(Index));

            tag.Name = updatedTag.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Details(int id)
        {
            if (id<= 0) return BadRequest();

            Tag tag = await _context.Tags
                .Include(t => t.ProductTags).ThenInclude(pt => pt.Product).ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null) return NotFound();

            return View(tag);      

        }
    }
}
