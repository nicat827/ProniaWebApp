using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = _context.Products
                .Include(product => product.Category)
                .Include(product => product.Images)
                .FirstOrDefault(p => p.Id == id);   

            if (product == null) return NotFound();

            List<Product> similarProducts = _context.Products
                .Include(product => product.Images)
                .Where(p => p.CategoryId == product.CategoryId && product.Id != p.Id).ToList();
            
            ProductVM productVM = new ProductVM
            {
                Product = product,
                SimilarProducts = similarProducts
            };
            return View(productVM);
        }
    }
}
