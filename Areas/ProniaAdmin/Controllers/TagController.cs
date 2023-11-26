using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
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
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = await _context.Tags.AnyAsync(t => t.Name == tagVM.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Tag with this name already exist!");
                return View();
            }
            Tag tag = new Tag { Name = tagVM.Name };
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
            UpdateTagVM tagVM = new UpdateTagVM { Name = tag.Name };
            return View(tagVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM updatedTagVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedTagVM);
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();

            bool isExist = await _context.Tags.AnyAsync(t => t.Name == updatedTagVM.Name.Trim() && t.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This tag already exist!");
                return View(updatedTagVM);
            }

            if (tag.Name == updatedTagVM.Name) return RedirectToAction(nameof(Index));

            tag.Name = updatedTagVM.Name;
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
