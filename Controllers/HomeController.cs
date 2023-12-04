using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            List<Slider> sliders = await _context.Sliders.OrderBy(s => s.Order).ToListAsync();
            List<Product> products = await _context.Products.OrderByDescending(p => p.Id).Take(10).Include(p => p.Images.Where(pi => pi.Type != ImageType.All)).Where(p => p.IsDeleted == false).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Sliders=sliders,
                Products = products

            };
            return View(homeVM);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
