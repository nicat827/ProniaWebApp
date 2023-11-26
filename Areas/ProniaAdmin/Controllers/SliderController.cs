using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
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

        public async Task<IActionResult> Create(CreateSliderVM sliderVM)
        {
            if (!ModelState.IsValid) return View();


            if (!sliderVM.Photo.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("Photo", "Please make sure you choose photo!");
                return View();
            }

            if (!sliderVM.Photo.IsValidSize(2, FileSize.Megabite))
            {
                ModelState.AddModelError("Photo", "Photo size must be less than 2MB!");
                return View();

            }
            Slider slider = new Slider
            {
                Title = sliderVM.Title,
                SubTitle = sliderVM.SubTitle,
                Description = sliderVM.Description,
                Order = sliderVM.Order,

            };
            slider.ImageURL = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slider");

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
            UpdateSliderVM sliderVM = new UpdateSliderVM
            {
                Title = slider.Title,
                SubTitle = slider.SubTitle,
                Description = slider.Description,
                ImageURL = slider.ImageURL,
                Order = slider.Order,

            };
            return View(sliderVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateSliderVM newSliderVM)
        {

            if (!ModelState.IsValid)
            {
                return View(newSliderVM);
            }

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();

            if (newSliderVM.Photo is not  null)
            {

                if (!newSliderVM.Photo.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("Photo", "Please make sure you choose photo!");
                    return View(newSliderVM);
                }

                if (!newSliderVM.Photo.IsValidSize(2, FileSize.Megabite))
                {
                    ModelState.AddModelError("Photo", "Photo size must be less than 2MB!");
                    return View(newSliderVM);

                }

                slider.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "slider");

                slider.ImageURL = await newSliderVM.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slider");

            }

            slider.Title = newSliderVM.Title;
            slider.Description = newSliderVM.Description;
            slider.SubTitle = newSliderVM.SubTitle;
            slider.Order = newSliderVM.Order;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
