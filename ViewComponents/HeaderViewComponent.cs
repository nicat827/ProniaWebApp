using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using  System.Security.Claims;

using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.ViewModels;
using System.Security.Claims;

namespace Pronia.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {

        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            List<BasketItemVM> items = new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users
                    .Include(a => a.BasketItems)
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                    .FirstOrDefaultAsync(a => a.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (user is null) return View(new HeaderVM { BasketItems = items, Settings = settings });   
                foreach (BasketItem basketItem in user.BasketItems)
                {
                    items.Add(new BasketItemVM
                    {
                        Id = basketItem.ProductId,
                        Count = basketItem.Count,
                        Name = basketItem.Product.Name,
                        ImageUrl = basketItem.Product.Images[0].ImageURL,
                        Price = basketItem.Product.Price,
                        Subtotal = basketItem.Product.Price * basketItem.Count

                    });
                }
            }
            else {
                string cookies = Request.Cookies["basket"];
                if (cookies == null) return View(new HeaderVM { BasketItems = items, Settings = settings });
                List<AddCooikesBasketVM> deserializedItems = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                bool HasDeletedProductInCookie = false;
                foreach (AddCooikesBasketVM item in deserializedItems)
                {
                    Product product = await _context.Products.Include(p => p.Images.Where(pi => pi.Type == ImageType.Main)).FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product is not null)
                    {
                        items.Add(new BasketItemVM
                        {
                            Id = product.Id,
                            Count = item.Count,
                            Price = product.Price,
                            Name = product.Name,
                            Subtotal = product.Price * item.Count,
                            ImageUrl = product.Images[0].ImageURL

                        });
                    }
                    else
                    {
                        deserializedItems.Remove(item);
                        if (!HasDeletedProductInCookie) HasDeletedProductInCookie = true;
                    };

                }

                if (HasDeletedProductInCookie)
                {
                    _http.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(deserializedItems));
                }
            }
           

            return View(new HeaderVM { BasketItems = items, Settings = settings});


        }

      
    }
}
