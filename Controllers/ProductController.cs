using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
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

        public async  Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = _context.Products
                .Include(product => product.Category)
                .Include(product => product.Images)
                .Include(product => product.ProductColors).ThenInclude(pc => pc.Color)
                .Include(product => product.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(product => product.ProductTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefault(p => p.Id == id);   

            if (product == null) return NotFound();

            List<Product> similarProducts =  await _context.Products
                .Include(prod => prod.Images.Where(i => i.Type != ImageType.All))
                .Where(p => p.CategoryId == product.CategoryId && product.Id != p.Id && p.IsDeleted == false).ToListAsync();

            ProductVM productVM = new ProductVM
            {
                Product = product,
                SimilarProducts = similarProducts
            };
            return View(productVM);
        }
    }
}
