using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;

namespace Pronia.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(ProductSort sortType)
        {
            switch (sortType)
            {
                case ProductSort.Name:
                    List<Product> products = await _context.Products.Take(8).OrderBy(p => p.Name).Include(p => p.Images.Where(i => i.Type != ImageType.All)).ToListAsync();
                    return View(products);
                case ProductSort.Price:
                    return View(await _context.Products.Take(8).OrderByDescending(p => p.Price).Include(p => p.Images.Where(i => i.Type != ImageType.All)).ToListAsync());
                case ProductSort.New:
                    return View(await _context.Products.Take(8).OrderByDescending(p => p.Id).Include(p => p.Images.Where(i => i.Type != ImageType.All)).ToListAsync());
                default: return View(new List<Product>());
            }
        }
    }
}
