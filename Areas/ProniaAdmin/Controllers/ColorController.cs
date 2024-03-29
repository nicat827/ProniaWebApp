﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
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
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View();
            bool isExist = await _context.Colors.AnyAsync(c => c.Name == colorVM.Name.Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This color already exist!");
                return View();
            }
            Color color = new Color { Name = colorVM.Name };
            await _context.Colors.AddAsync(color);

            await _context.SaveChangesAsync();
            TempData["success"] = "Color succesfully created!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            if (id <= 0) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);
            if (color == null) return NotFound();
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = await _context.Colors
                .Include(c => c.ProductColors).ThenInclude(pc => pc.Product).ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (color is null) return NotFound();
            return View(color);

        }


        public async Task<IActionResult> Update(int id)
        {
            if (id<= 0) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color is null) return NotFound();
            UpdateColorVM colorVM = new UpdateColorVM { Name = color.Name };
            return View(colorVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View(colorVM);

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color is null) return NotFound();

            bool isExisted = await _context.Colors.AnyAsync(p => p.Id != id && p.Name.Trim() == colorVM.Name.Trim());
            if (isExisted)
            {
                ModelState.AddModelError("Name", "Color with this name already exist!");
                return View(colorVM);
            }

            color.Name = colorVM.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


    }
}
