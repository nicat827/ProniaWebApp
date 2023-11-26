using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToListAsync();
                
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            return View(new CreateProductVM { Categories = await GetCategories() });

        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await GetCategories();
                return View(productVM);
            }
            // main photo validation

            if (!productVM.MainPhoto.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategories();
                return View(productVM);
            }

            if (!productVM.MainPhoto.IsValidSize(200, FileSize.Kilobite))
            {
                ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 200kB!");
                productVM.Categories = await GetCategories();
                return View(productVM);
            }

            // hover photo validation
            if (!productVM.HoverPhoto.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategories();
                return View(productVM);
            }

            if (!productVM.HoverPhoto.IsValidSize(200, FileSize.Kilobite))
            {
                ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 200kB!");
                productVM.Categories = await GetCategories();
                return View(productVM);
            }


            // validate others photo
            if (productVM.OthersPhoto is not null)
            {
                foreach (IFormFile photo in productVM.OthersPhoto)
                {
                    if (!photo.IsValidType(FileType.Image))
                    {
                        ModelState.AddModelError("OthersPhoto", "Please, make sure, you uploaded an image!");
                        productVM.Categories = await GetCategories();
                        return View(productVM);
                    }


                    if (!photo.IsValidSize(200, FileSize.Kilobite))
                    {
                        ModelState.AddModelError("OthersPhoto", "Photo size can't be bigger than 200kB!");
                        productVM.Categories = await GetCategories();
                        return View(productVM);
                    }

                }
            }




            bool isExistCategory = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!isExistCategory)
            {
                ModelState.AddModelError("CategoryId", "Please, make sure you choosed an exist category!");
                productVM.Categories = await GetCategories();
                return View(productVM);
            }

            List<ProductImage> images = new List<ProductImage>();

            Product product = new Product
            {
                Name = productVM.Name.Trim(),
                ShortDescription = productVM.ShortDescription.Trim(),
                Description = productVM.Description.Trim(),
                SKU = productVM.SKU.Trim(),
                Price = (int)productVM.Price,
                CategoryId = productVM.CategoryId,
                IsDeleted = false,

            };

            ProductImage mainImage = new ProductImage
            {
                Type = ImageType.Main,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                Product = product
            };
            ProductImage hoverImage = new ProductImage
            {
                Type = ImageType.Hover,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                Product = product
            };
            List<ProductImage> othersImages = new List<ProductImage>();

            if (productVM.OthersPhoto is not null)
            {
                foreach (IFormFile photo in productVM.OthersPhoto)
                {
                    ProductImage image = new ProductImage
                    {
                        Type = ImageType.All,
                        ImageURL = await photo.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                        Product = product

                    };
                    othersImages.Add(image);
                }
            }

            


            images.Add(mainImage);
            images.Add(hoverImage);
            images.AddRange(othersImages);


            product.Images = images;

            await _context.Products.AddAsync(product);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(product);
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
           
            if (product.Images is not null)
            {
                foreach (ProductImage image in product.Images)
                {
                    image.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");
                }
            }
            
            
          
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                SKU = product.SKU,
                Images = product.Images,
                CategoryId = product.CategoryId,
                Categories = await GetCategories()

            };

            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                Product findedProduct = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == id);

                productVM.Categories = await GetCategories();
                productVM.Images = findedProduct.Images;
                return View(productVM);
            }
            
            Product product = await _context.Products
               .Include(p => p.Images)
               .FirstOrDefaultAsync(p => p.Id == id);

            bool res = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!res)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist!");
                productVM.Categories = await GetCategories();
                productVM.Images = product.Images;

                return View(productVM);
            }

            if (product == null) return NotFound();

            if (productVM.MainPhoto is not null)
            {

                if (!productVM.MainPhoto.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");
                    productVM.Categories = await GetCategories();
                    productVM.Images = product.Images;
                    return View(productVM);
                }

                if (!productVM.MainPhoto.IsValidSize(200, FileSize.Kilobite))
                {
                    ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 200kB!");
                    productVM.Categories = await GetCategories();
                    productVM.Images = product.Images;

                    return View(productVM);
                }

                ProductImage mainImage = new ProductImage
                {
                    Type = ImageType.Main,
                    ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                    Product = product
                };
                product.Images.FirstOrDefault(i => i.Type == ImageType.Main).ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");

                int idx = product.Images.IndexOf(product.Images.FirstOrDefault(i => i.Type == ImageType.Main));
                product.Images[idx] = mainImage;
                await _context.SaveChangesAsync();


            }

            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                    productVM.Categories = await GetCategories();
                    productVM.Images = product.Images;

                    return View(productVM);
                }

                if (!productVM.HoverPhoto.IsValidSize(200, FileSize.Kilobite))
                {
                    ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 200kB!");
                    productVM.Categories = await GetCategories();
                    productVM.Images = product.Images;

                    return View(productVM);
                }

                ProductImage hoverImage = new ProductImage
                {
                    Type = ImageType.Hover,
                    ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                    Product = product
                };
                product.Images.FirstOrDefault(i => i.Type == ImageType.Hover).ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");

                int idx = product.Images.IndexOf(product.Images.FirstOrDefault(i => i.Type == ImageType.Hover));
                product.Images[idx] = hoverImage;
                await _context.SaveChangesAsync();

            }

            if (productVM.OthersPhoto is not null)
            {

                foreach (IFormFile photo in productVM.OthersPhoto)
                {

                    if (!photo.IsValidType(FileType.Image))
                    {
                        ModelState.AddModelError("OthersPhoto", "Please, make sure, you uploaded an image!");
                        productVM.Categories = await GetCategories();
                        productVM.Images = product.Images;

                        return View(productVM);
                    }


                    if (!photo.IsValidSize(200, FileSize.Kilobite))
                    {
                        ModelState.AddModelError("OthersPhoto", "Photo size can't be bigger than 200kB!");
                        productVM.Categories = await GetCategories();
                        productVM.Images = product.Images;

                        return View(productVM);
                    }
                }

                List<ProductImage> othersImages = new List<ProductImage>();

                foreach (IFormFile photo in productVM.OthersPhoto)
                {
                    ProductImage otherImage = new ProductImage
                    {
                        Type = ImageType.All,
                        ImageURL = await photo.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                        Product = product
                    };

                    othersImages.Add(otherImage);
                }
                
                foreach (ProductImage oldImage in product.Images.Where(i => i.Type == ImageType.All))
                {
                    oldImage.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");
                    product.Images.Remove(oldImage);
                    await _context.SaveChangesAsync();
                }

                product.Images.AddRange(othersImages);
                await _context.SaveChangesAsync();

            }

            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.ShortDescription = productVM.ShortDescription;
            product.SKU = productVM.SKU;
            product.Price = (decimal) productVM.Price;
            product.CategoryId = productVM.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<List<Category>> GetCategories()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }
    }
}
