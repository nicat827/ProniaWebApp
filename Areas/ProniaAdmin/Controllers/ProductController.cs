﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Exceptions;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;

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
        [Authorize(Roles = "Admin, Moderator")]

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) throw new BadRequestException("Id is not valid!");
            // isDeleted false olan productlarin countu ( hamsi yox)
            int productsCount = await _context.Products.Where(p => p.IsDeleted == false).CountAsync();
            int totalPages = (int)Math.Ceiling((double)productsCount / 3);
            if (page > totalPages) return BadRequest();
            //wheri yuxaarda vermek lazimdir ki , isDeleted true olsa duzgun islemir ( meselen: 2 -ci seyfede 3 dene product cekmelidir
            //, where asagida olsa hamsin cekecey ve birin gostermiyecey)
            List<Product> products = await _context.Products
                .Where(p => p.IsDeleted == false)
                .Skip((page - 1) * 3).Take(3)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToListAsync();
                
            return View(new PaginationVM<Product>
            {
                CurrentPage = page,
                TotalPage = totalPages,
                Items = products
            });
        }

        [Authorize(Roles = "Admin, Moderator")]

        public async Task<IActionResult> Create()
        {
            return View(new CreateProductVM { 
                Categories = await GetCategoriesAsync() ,
                Tags = await GetTagsAsync(),
                Colors = await GetColorsAsync(),
                Sizes = await GetSizesAsync()
                
            });



        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            // main photo validation

            if (!productVM.MainPhoto.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }

            if (!productVM.MainPhoto.IsValidSize(200, FileSize.Kilobite))
            {
                ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 200kB!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }

            // hover photo validation
            if (!productVM.HoverPhoto.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }

            if (!productVM.HoverPhoto.IsValidSize(200, FileSize.Kilobite))
            {
                ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 200kB!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }


            bool isExistCategory = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!isExistCategory)
            {
                ModelState.AddModelError("CategoryId", "Please, make sure you choosed an exist category!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            if (productVM.TagIds is not null)
            {
                foreach (int tagId in productVM.TagIds)
                {
                    bool isExistTag = await _context.Tags.AnyAsync(t => t.Id == tagId);
                    if (!isExistTag)
                    {
                        ModelState.AddModelError("TagIds", "Please, make sure you choosed an exist tag!");
                        productVM.Categories = await GetCategoriesAsync();
                        productVM.Tags = await GetTagsAsync();
                        productVM.Colors = await GetColorsAsync();
                        productVM.Sizes = await GetSizesAsync();
                        return View(productVM);

                    }
                }
            }
            
            
            foreach (int colorId in productVM.ColorIds)
            {
                bool isExistColor = await _context.Colors.AnyAsync(t => t.Id == colorId);
                if (!isExistColor)
                {
                    ModelState.AddModelError("ColorIds", "Please, make sure you choosed an exist color!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    return View(productVM);

                }
            }
            
           
            foreach (int sizeId in productVM.SizeIds)
            {
                bool isExistSize = await _context.Colors.AnyAsync(t => t.Id == sizeId);
                if (!isExistSize)
                {
                    ModelState.AddModelError("SizeIds", "Please, make sure you choosed an exist size!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    return View(productVM);

                }
            }
            
            
            Product product = new Product
            {
                Name = productVM.Name.Trim(),
                ShortDescription = productVM.ShortDescription.Trim(),
                Description = productVM.Description.Trim(),
                SKU = productVM.SKU.Trim(),
                Price = (int)productVM.Price,
                CategoryId = productVM.CategoryId,
                Images = new List<ProductImage>(),
                ProductTags = new List<ProductTag>(),
                ProductSizes = new List<ProductSize>(),
                ProductColors = new List<ProductColor>(),
                IsAvailable = true,
                IsDeleted = false,

            };

            // adding photos into DB

            product.Images.Add(new ProductImage
            {
                Type = ImageType.Main,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product")
            });

            product.Images.Add(new ProductImage
            {
                Type = ImageType.Hover,
                ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product")
            });

            if (productVM.OthersPhoto is not null)
            {
                TempData["ErrorMessages"] = "";

                foreach (IFormFile photo in productVM.OthersPhoto)
                {
                    if (!photo.IsValidType(FileType.Image))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because type is not valid!</p>";
                        continue;
                    }


                    if (!photo.IsValidSize(400, FileSize.Kilobite))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because size must be lower than 400kB!</p>";

                        continue;
                    }

                    product.Images.Add(new ProductImage
                    {
                        Type = ImageType.All,
                        ImageURL = await photo.CreateFileAsync(_env.WebRootPath, "uploads", "product")
                    });
                }
            }


            // adding tags, colors, sizes into DB
            if (productVM.TagIds is not null) productVM.TagIds.ForEach(tId => product.ProductTags.Add(new ProductTag { TagId = tId }));
            productVM.ColorIds.ForEach(cId => product.ProductColors.Add(new ProductColor { ColorId = cId }));
            productVM.SizeIds.ForEach(sId => product.ProductSizes.Add(new ProductSize { SizeId = sId }));
            await _context.Products.AddAsync(product);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin, Moderator")]

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

        [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin, Moderator")]

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
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
                IsAvilable = product.IsAvailable,
                Categories = await GetCategoriesAsync(),
                Tags = await GetTagsAsync(),
                Colors = await GetColorsAsync(),
                Sizes = await GetSizesAsync(),
                TagIds = product.ProductTags.Select(p => p.TagId).ToList(),
                ColorIds = product.ProductColors.Select(p => p.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(p => p.SizeId).ToList()

            };

            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (!ModelState.IsValid)
            {
                productVM.Categories = await GetCategoriesAsync();
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                productVM.Images = product.Images;
                return View(productVM);
            }

         

            if (productVM.MainPhoto is not null)
            {

                if (!productVM.MainPhoto.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");

                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    productVM.Images = product.Images;
                    return View(productVM);
                }

                if (!productVM.MainPhoto.IsValidSize(200, FileSize.Kilobite))
                {
                    ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 200kB!");

                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    productVM.Images = product.Images;
                    return View(productVM);
                }

               


            }

            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Images = product.Images;

                    return View(productVM);
                }

                if (!productVM.HoverPhoto.IsValidSize(200, FileSize.Kilobite))
                {
                    ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 200kB!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Images = product.Images;

                    return View(productVM);
                }


            }

         



            bool res = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!res)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Images = product.Images;
                productVM.Tags = await GetTagsAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }


            if (productVM.TagIds is not null)
            {
                foreach (int tagId in productVM.TagIds)
                {
                    bool isExistTag = await _context.Tags.AnyAsync(t => t.Id == tagId);
                    if (!isExistTag)
                    {
                        ModelState.AddModelError("TagIds", "Please, make sure you choosed an exist tag!");
                        productVM.Categories = await GetCategoriesAsync();
                        productVM.Tags = await GetTagsAsync();
                        productVM.Colors = await GetColorsAsync();
                        productVM.Sizes = await GetSizesAsync();
                        productVM.Images = product.Images;

                        return View(productVM);

                    }
                }
            }
            
            foreach (int colorId in productVM.ColorIds)
            {
                bool isExistColor = await _context.Colors.AnyAsync(t => t.Id == colorId);
                if (!isExistColor)
                {
                    ModelState.AddModelError("ColorIds", "Please, make sure you choosed an exist color!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    productVM.Images = product.Images;

                    return View(productVM);

                }
            }
            
          
            foreach (int sizeId in productVM.SizeIds)
            {
                bool isExistSize = await _context.Sizes.AnyAsync(t => t.Id == sizeId);
                if (!isExistSize)
                {
                    ModelState.AddModelError("SizeIds", "Please, make sure you choosed an exist size!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Tags = await GetTagsAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Sizes = await GetSizesAsync();
                    productVM.Images = product.Images;

                    return View(productVM);

                }
            }
            
            // delete from db canceled tags

            if (productVM.TagIds is not null)
            {
                foreach (ProductTag pTag in product.ProductTags)
                {
                    if (!productVM.TagIds.Exists(id => id == pTag.TagId))
                    {
                        _context.ProductTags.Remove(pTag);
                    }
                }
                // add into productTags new selected tags
                foreach (int tagId in productVM.TagIds)
                {
                    if (!product.ProductTags.Exists(pt => pt.TagId == tagId))
                    {
                        product.ProductTags.Add(new ProductTag { TagId = tagId });
                    }
                }
            }
            else product.ProductTags = null;
            


            foreach (ProductColor pColor in product.ProductColors)
            {
                if (!productVM.ColorIds.Exists(id => id == pColor.ColorId))
                {
                    _context.ProductColors.Remove(pColor);
                }
            }
            foreach (int colorId in productVM.ColorIds)
            {
                if (!product.ProductColors.Exists(pc => pc.ColorId == colorId))
                {
                    product.ProductColors.Add(new ProductColor { ColorId = colorId });
                }
            }

            foreach (ProductSize pSize in product.ProductSizes)
            { 
                if (!productVM.SizeIds.Exists(id => id == pSize.SizeId))
                {
                    _context.ProductSizes.Remove(pSize);
                }
            }

            foreach (int sizeId in productVM.SizeIds)
            {
                if (!product.ProductSizes.Exists(ps => ps.SizeId == sizeId))
                {
                    product.ProductSizes.Add(new ProductSize { SizeId = sizeId });
                }
            }


            if (productVM.MainPhoto is not null)
            {
                ProductImage mainImage = new ProductImage
                {
                    Type = ImageType.Main,
                    ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product")
                };
                ProductImage oldImage = product.Images.FirstOrDefault(i => i.Type == ImageType.Main);
                oldImage.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");
                product.Images.Remove(oldImage);
                product.Images.Add(mainImage);
            }
            if (productVM.HoverPhoto is not null)
            {

                ProductImage hoverImage = new ProductImage
                {
                    Type = ImageType.Hover,
                    ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product"),
                };
                ProductImage oldImage = product.Images.FirstOrDefault(i => i.Type == ImageType.Hover);
                oldImage.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product");
                product.Images.Remove(oldImage);
                product.Images.Add(hoverImage);
            }
            if (productVM.ImageIds is null) productVM.ImageIds = new List<int>();
            List<ProductImage> removeable = product.Images.Where(i => !productVM.ImageIds.Exists(id => id == i.Id) && i.Type == ImageType.All).ToList();
            removeable.ForEach(item => item.ImageURL.DeleteFile(_env.WebRootPath, "uploads", "product"));
            _context.ProductImages.RemoveRange(removeable);

            if (productVM.OthersPhoto is not null)
            {   
                foreach (IFormFile photo in productVM.OthersPhoto)
                {
                    if (!photo.IsValidType(FileType.Image))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because type is not valid!</p>";
                        continue;
                    }


                    if (!photo.IsValidSize(200, FileSize.Kilobite))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because size bigger than we except (400kB) :(</p>";
                        continue;
                    }

                    product.Images.Add(new ProductImage { ImageURL = await photo.CreateFileAsync(_env.WebRootPath, "uploads", "product"), Type=ImageType.All });
                }

            }

            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.ShortDescription = productVM.ShortDescription;
            product.SKU = productVM.SKU;
            product.Price = (decimal) productVM.Price;
            product.CategoryId = productVM.CategoryId;
            product.IsAvailable = productVM.IsAvilable;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<List<Color>> GetColorsAsync()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<List<Size>> GetSizesAsync()
        {
            return await _context.Sizes.ToListAsync();
        }

       
    }
}
