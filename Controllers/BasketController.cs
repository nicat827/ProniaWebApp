using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.ViewModels;


namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {   List<BasketItemVM> items = new List<BasketItemVM>();
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
           


            
            return View(items);
        }

        public async Task<IActionResult> Add(int id, string ctrl, string act, string redId)
        {
            if (id <= 0) return BadRequest();
            Product product =  await _context.Products.FirstOrDefaultAsync(p => p.Id ==  id);

            if (product == null) return NotFound();

            string cookies = Request.Cookies["basket"];
            if (cookies.IsNullOrEmpty())
            {
                List<AddCooikesBasketVM> items = new List<AddCooikesBasketVM> { new AddCooikesBasketVM
                {
                    Count = 1,
                    ProductId = id
                }};

                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
                return RedirectToAction(act, ctrl, new { id = redId });
            }
            else
            {

                List<AddCooikesBasketVM> items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
                AddCooikesBasketVM existedItem = items.FirstOrDefault(i => i.ProductId == id);
                if (existedItem is not null)
                {
                    existedItem.Count++;
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
                    return RedirectToAction(act, ctrl, new {id = redId});
                }

                items.Add(new AddCooikesBasketVM { Count = 1, ProductId = id });
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
                return RedirectToAction(act, ctrl, new {id = redId});


            }
        }

        public async Task<IActionResult> Decreese(int id)
        {
            if (id <= 0) return BadRequest();
            string cookies = Request.Cookies["basket"];
            if (cookies.IsNullOrEmpty()) return NotFound();
           
            List<AddCooikesBasketVM> items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
            AddCooikesBasketVM changeItem = items.FirstOrDefault(i => i.ProductId == id);
            if (changeItem is null) return NotFound();
            if (changeItem.Count > 1) {
                changeItem.Count--;
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
                return RedirectToAction(nameof(Index));

            }
            items.Remove(changeItem);
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));

            return RedirectToAction(nameof(Index));
            

        }

        public async Task<IActionResult> Remove(int id, string ctrl, string act, string redId)
        {
            if (id <= 0) return BadRequest();

            string cookies = Request.Cookies["basket"];
            if (cookies.IsNullOrEmpty()) return NotFound();

            List<AddCooikesBasketVM> items = JsonConvert.DeserializeObject<List<AddCooikesBasketVM>>(cookies);
            AddCooikesBasketVM removeItem = items.FirstOrDefault(i => i.ProductId == id);
            if (removeItem is null) return NotFound();
            items.Remove(removeItem);
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));
            return RedirectToAction(act, ctrl, new {id = redId});
        }

        public IActionResult Test()
        {
            return Content(Request.Cookies["basket"]);
        }
    }
}
