using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders
                
                .ToListAsync();


            return View(sliders);
        }

        

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return NotFound();
            Slider slider = await _context.Sliders
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slider == null) return NotFound();

            return View(slider);

        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            if (slider.Photo is null)
            {
                ModelState.AddModelError("Photo", "Photo is required!");
                return View();
            }

            if (!slider.Photo.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("Photo", "Please make sure you choose photo!");
                return View();
            }

            if (!slider.Photo.IsValidSize(2, FileSize.Megabite))
            {
                ModelState.AddModelError("Photo", "Photo size must be less than 2MB!");
                return View();

            }
            slider.ImageURL = await slider.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slider");

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));




        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();
            slider.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "slider");

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, Slider newSlider)
        {

            if (!ModelState.IsValid)
            {
                return View(newSlider);
            }

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();

            if (newSlider.Photo is not  null)
            {

                if (!newSlider.Photo.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("Photo", "Please make sure you choose photo!");
                    return View();
                }

                if (!newSlider.Photo.IsValidSize(2, FileSize.Megabite))
                {
                    ModelState.AddModelError("Photo", "Photo size must be less than 2MB!");
                    return View();

                }

                slider.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "slider");

                slider.ImageURL = await newSlider.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slider");

            }

            slider.Title = newSlider.Title;
            slider.Description = newSlider.Description;
            slider.SubTitle = newSlider.SubTitle;
            slider.Order = newSlider.Order;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
