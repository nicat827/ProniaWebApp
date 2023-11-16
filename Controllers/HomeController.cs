using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;
using Pronia.Models;
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
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.OrderBy(s => s.Order).ToList();
            List<Product> products = _context.Products.OrderByDescending(s => s.Id).ToList();

            HomeVM homeVM = new HomeVM
            {
                Sliders=sliders,
                Products=products,
                NewProducts = products.Take(8).ToList(),
            };
            return View(homeVM);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
