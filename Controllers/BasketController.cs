using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.ViewModels;
using System.Security.Claims;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {   List<BasketItemVM> items = new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users
                     .Include(u => u.BasketItems)
                     .ThenInclude(bi => bi.Product)
                     .ThenInclude(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                     .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user == null) return NotFound();

                foreach (BasketItem item in user.BasketItems)
                {
                    items.Add(new BasketItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        ImageUrl = item.Product.Images[0].ImageURL,
                        Count = item.Count,
                        Price = item.Product.Price,
                        Subtotal = item.Product.Price * item.Count

                    });
                }
            }
            else
            {
                string cookies = Request.Cookies["basket"];
                if (cookies.IsNullOrEmpty())
                {
                    return View(items);
                }

                List<AddCooikesBasketVM> deserializedCooikes = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                foreach (AddCooikesBasketVM item in deserializedCooikes)
                {
                    Product product = await _context.Products.Include(p => p.Images.Where(i => i.Type == ImageType.Main)).FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product is not null)
                    {
                        items.Add(new BasketItemVM
                        {
                            Count = item.Count,
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Subtotal = product.Price * item.Count,
                            ImageUrl = product.Images[0].ImageURL,


                        });
                    }
                }
            }

            return View(items);
        }

        public async Task<IActionResult> Add(int id, string? returnUrl)
        {
            if (id <= 0) return BadRequest();
            Product product =  await _context.Products.FirstOrDefaultAsync(p => p.Id ==  id);
            if (product == null) return NotFound();


            List<AddCooikesBasketVM> items = new List<AddCooikesBasketVM>();

            if (User.Identity.IsAuthenticated)
            {
               
                BasketItem item = await _context.BasketItems
                    .Include(bi => bi.Product)
                    .FirstOrDefaultAsync(bi => bi.ProductId == id && bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (item is null)
                {
                    await _context.BasketItems.AddAsync(new BasketItem
                    {
                        ProductId = product.Id,
                        AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Price = product.Price,
                        Count = 1,

                    });
                }
                else
                {
                    item.Count++;
                }

                await _context.SaveChangesAsync();
            }

            else {
                string cookies = Request.Cookies["basket"];
                if (cookies.IsNullOrEmpty())
                {
                    items.Add(new AddCooikesBasketVM
                    {
                        Count = 1,
                        ProductId = id
                    });

                }
                else
                {

                    items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                    AddCooikesBasketVM existedItem = items.FirstOrDefault(i => i.ProductId == id);
                    if (existedItem is not null)
                    {
                        existedItem.Count++;
                    }
                    else
                    {
                        items.Add(new AddCooikesBasketVM { Count = 1, ProductId = id });
                    }


                }

                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
            }
           
            if (returnUrl is not null) return Redirect(returnUrl);
            return RedirectToAction("Index", "Basket");
        }

        public async Task<IActionResult> Decrease(int id)
        {
            if (id <= 0) return BadRequest();
            if (User.Identity.IsAuthenticated)
            {
                BasketItem? item = await _context.BasketItems.FirstOrDefaultAsync(bi => bi.ProductId == id && bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (item == null) return NotFound();

                if (item.Count > 1)
                {
                    item.Count--;

                }
                else
                {
                    _context.BasketItems.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                string cookies = Request.Cookies["basket"];
                if (cookies.IsNullOrEmpty()) return NotFound();

                List<AddCooikesBasketVM> items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                AddCooikesBasketVM changeItem = items.FirstOrDefault(i => i.ProductId == id);
                if (changeItem is null) return NotFound();
                if (changeItem.Count > 1)
                {
                    changeItem.Count--;
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
                    return RedirectToAction(nameof(Index));

                }
                items.Remove(changeItem);
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
            }
           

            return RedirectToAction(nameof(Index));
            

        }

        public async Task<IActionResult> Remove(int id,string? returnUrl)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(bi => bi.ProductId ==  id && bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (basketItem is null) return NotFound();
                _context.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                string cookies = Request.Cookies["basket"];
                if (cookies.IsNullOrEmpty()) return NotFound();

                List<AddCooikesBasketVM> items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                AddCooikesBasketVM removeItem = items.FirstOrDefault(i => i.ProductId == id);
                if (removeItem is null) return NotFound();
                items.Remove(removeItem);
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
            }
           
            if (returnUrl is not null) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Test()
        {
            return Content(Request.Cookies["basket"]);
        }
    }
}
