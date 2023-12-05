using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;
using System.Configuration;

namespace Pronia.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM userVM)
        {
            if (!ModelState.IsValid) {
                return View(userVM);
            }
            bool isValid = false;
            foreach (Gender gender in  Enum.GetValues(typeof(Gender)))
            {
                if (gender == userVM.Gender)
                {
                    isValid = true;
                    break;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("Gender", "Gender is not valid!");
                return View(userVM);

            }

         

            RegexStringValidator validator = new RegexStringValidator("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            try
            {
                validator.Validate(userVM.Email);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Email", "Email is not valid!");
                return View(userVM);
            }

            AppUser user = new AppUser
            {
                Name = userVM.Name.Capitalize(),
                Surname = userVM.Surname.Capitalize(),
                Email = userVM.Email,
                UserName = userVM.Username,
                Gender = userVM.Gender

            };
            IdentityResult res = await _userManager.CreateAsync(user, userVM.Password);

            if (!res.Succeeded)
            {
                foreach (IdentityError error in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(userVM);

            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> LogOut()
        {
            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
