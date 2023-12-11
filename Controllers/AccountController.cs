using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using  Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Interfaces;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Enums.AuthEnums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;
using System.Configuration;
using System.Security.Claims;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return NotFound();
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

            //await _signInManager.SignInAsync(user, isPersistent: false);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string action = Url.Action(nameof(SuccessConfirm), "Account", new { token, Email = user.Email }, Request.Scheme);
            string body = $"<a style='width:80px; height:50px; background-color:blue;color:white;border:1px solid white; border-radius:8px; text-decoration:none; font-weight:700;' href={action}>Confirm Email</a>";
            await _emailService.SendEmailAsync(user.Email, body, "Email Confirmation", true);
            ViewBag.ConfirmEmailModal = true;
            return View();

        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return NotFound();

            return View();
        }

        public async Task<ActionResult> SuccessConfirm(string token, string email)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return NotFound();
            var res = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!res.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(appUser, false);
            TempData.Add("confirm-success", true);
            return RedirectToAction("Index", "Home");
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

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email, before you can sign in!");
                return View(userVM);


            }

            if (!res.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username, Email or password is incorrect!");
                return View();

            }

            if (Request.Cookies["basket"] is not null)
            {
                Response.Cookies.Delete("basket");
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

        [Authorize]
        public async Task<IActionResult> Index() 
        {
            AppUser? user = await _userManager.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(o => o.Product)
                .ThenInclude(p => p.Images.Where(i => i.Type == ImageType.Main))
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
         
            return View(new UserDetailsVM
            { 
                Orders = user.Orders,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Username = user.UserName,
                Gender = user.Gender
            });
        }

    }
}
