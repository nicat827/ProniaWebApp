using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using  Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Enums.AuthEnums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;
using System.Configuration;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM userVM, string? returnUrl)
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
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());

            await _signInManager.SignInAsync(user, isPersistent: false);
            if (returnUrl is null)
            {
                return RedirectToAction("Index", "Home");

            }

            return Redirect(returnUrl);

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(UserLoginVM userVM, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await _userManager.FindByNameAsync(userVM.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userVM.UsernameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Username, Email or password is incorrect!");
                    return View();
                }
            }

            var  res = await _signInManager.PasswordSignInAsync(user, userVM.Password, userVM.IsRemembered, true);

            if (res.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts, try later!");
                userVM.IsLocked = true;
                return View(userVM);
            }

            if (!res.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username, Email or password is incorrect!");
                return View();

            }               


            
            if (returnUrl == null)
            {
                return RedirectToAction("Index", "Home");

            }

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> LogOut(string? returnUrl)
        {
            
            await _signInManager.SignOutAsync();
            if (returnUrl is null)
            {
                return RedirectToAction("Index", "Home");

            }

            return Redirect(returnUrl);

        }

        public async Task<IActionResult> CreateRoles()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    });
                }
               
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
