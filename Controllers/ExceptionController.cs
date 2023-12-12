using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class ExceptionController : Controller
    {
        public IActionResult Index(string exMessage= "Something went wrong!")
        {
            return View(new ExceptionVM {Message = exMessage});
        }
    }
}
