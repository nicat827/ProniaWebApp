using Microsoft.AspNetCore.Mvc;
using Pronia.Models;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Product> products = new List<Product> {
                new Product
                {
                    Id = 1,
                    Name = "American Marigold",
                    Price = 23.45m,
                    Image = "1-2-270x300.jpg"

                },
                new Product
                {
                    Id = 2,
                    Name = "Black Eyed Susan",
                    Price = 25.45m,
                    Image = "1-3-270x300.jpg"

                },  
                new Product
                {
                    Id = 3,
                    Name = "Bleeding Heart",
                    Price = 30.45m,
                    Image = "1-4-270x300.jpg"

                },



            };
            return View(products);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
