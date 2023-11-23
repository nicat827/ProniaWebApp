using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
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

            if (slider.Photo is null)
            {
                ModelState.AddModelError("Photo", "Photo is required!");
                return View();
            }

            if (!slider.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Please make sure you choose photo!");
                return View();
            }

            if (slider.Photo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Photo", "Photo size must be less than 2MB!");
                return View();

            }

            FileStream file = new FileStream(@"C:\\Users\\nicim\\OneDrive\\Desktop\\Pronia\\wwwroot\\uploads\\slider\\" + slider.Photo.FileName, FileMode.Create);
            await slider.Photo.CopyToAsync(file);
            slider.ImageURL = slider.Photo.FileName;

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));




        }
    }
}
