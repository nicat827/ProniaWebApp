using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.ViewModels;
using static System.Net.WebRequestMethods;

namespace Pronia.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {

        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            string cookies = Request.Cookies["basket"];
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (cookies == null) return View(new HeaderVM { BasketItems = items, Settings = settings});
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
                else {
                    deserializedItems.Remove(item);
                    if (!HasDeletedProductInCookie) HasDeletedProductInCookie = true;
                };

            }

            if (HasDeletedProductInCookie)
            {
                _http.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(deserializedItems));
            }

            return View(new HeaderVM { BasketItems = items, Settings = settings});


        }

      
    }
}
