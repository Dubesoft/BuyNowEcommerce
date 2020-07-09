using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models;
using BuyNowEcommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(ApplicationDbContext db,
                                 UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

         [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
           

            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                var userModel = new ApplicationUser
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(userModel, model.Password);

                if (result.Succeeded)
                {

                    await _signInManager.SignInAsync(userModel, isPersistent: false);
                    return Redirect("/Customer/Home/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(ReturnUrl))
                    {
                        Redirect(ReturnUrl);
                    }
                    else
                    {
                        var userResult = await _db.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();
                        HttpContext.Session.SetString(SD.UserId, userResult.Id);
                        return Redirect("/Customer/Home/Index");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }

            return RedirectToAction(nameof(Login));

        }
 
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}