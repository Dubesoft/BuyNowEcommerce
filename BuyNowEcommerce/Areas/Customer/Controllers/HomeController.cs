using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BuyNowEcommerce.Models;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BuyNowEcommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace BuyNowEcommerce.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext db, 
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Category = await _db.Category.ToListAsync(),
                Service = await _db.Services.ToListAsync()

            };

            var user = await _userManager.GetUserAsync(User);

            if(user != null)
            {
                var count = _db.Cart.Where(c => c.ApplicationUserId == user.Id).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssCartCount, count);
            }
            return View(IndexVM);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {

            //var UserId = HttpContext.Session.GetString(SD.UserId);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("ErrorOccured");
            }
            //var userId = await _userManager.FindByIdAsync(user.Id);
            //var user = await accountService.GetUser(_db, UserId);
            var model = new User
            {
                UserId = user.Id,
                Username = user.UserName,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                Picture = user.Picture,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserProfile(User model)
        {

            var files = HttpContext.Request.Form.Files;

            if (model == null)
            {
                return View("ErrorOccured");
            }
            else
            {
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    model.Picture = p1;
                }

                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {model.UserId} could not be found";
                    return View("ErrorOccured");
                }

                user.Id = model.UserId;
                user.UserName = model.Email;
                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.StreetAddress = model.StreetAddress;
                user.City = model.City;
                user.State = model.State;
                user.PostalCode = model.PostalCode;
                user.IsActive = true;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(UserProfile));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);

            }
        }



        [HttpGet]
        public async Task<IActionResult> ServiceDetail(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var service = await _db.Services.FindAsync(id);
            ServiceVm serviceVm = new ServiceVm()
            {
                CategoryList = await _db.Category.Where(s => s.Id == id).ToListAsync(),
                Services = await _db.Services.SingleOrDefaultAsync(s => s.Id == id)
            };

            if (serviceVm == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return View(serviceVm);
        }

        [HttpGet]
        public async Task<IActionResult> Cart(Cart cartModel)
        {
            var detailCart = new OrderDetailsCart
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var user = await _userManager.GetUserAsync(User);

            var cart = _db.Cart.Where(c => c.ApplicationUserId == user.Id);

            if(cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.Services = await _db.Services.FirstOrDefaultAsync(s => s.Id == list.ServicceId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.Services.Price * list.Count);

            }
            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }

            return View(detailCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(ServiceVm serviceVm)
        {
            
                var user = await _userManager.GetUserAsync(User);

                Cart cartFromDb = await _db.Cart.Where(c => c.ApplicationUserId == user.Id && c.ServicceId == serviceVm.Services.Id).FirstOrDefaultAsync();

                if(cartFromDb == null)
                {
                    var cartModel = new Cart()
                    {
                        ApplicationUserId = user.Id,
                        ServicceId = serviceVm.Services.Id,
                        Count = serviceVm.Cart.Count
                    };
                    await _db.Cart.AddAsync(cartModel);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + serviceVm.Cart.Count;
                }
                await _db.SaveChangesAsync();
                var count = _db.Cart.Where(c => c.ApplicationUserId == user.Id).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssCartCount, count);
                //return Redirect("/Customer/Home/Index");
                return RedirectToAction(nameof(Cart));
            
        }

        public IActionResult Privacy()
        {  
            return View();
        }

        public async Task<IActionResult> Blog()
        {
            var Post = await _db.Post.ToListAsync();
            return View(Post);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Service()
        {
            var Service = await _db.Services.Include(s => s.Category).ToListAsync();
            return View(Service);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string value)
        {
            if (value == null)
            {
                return NotFound();
            }

            var Category = await _db.Category.Where(c => c.Name == value).FirstOrDefaultAsync();

            if(Category == null)
            {
                return NotFound();
            }

            var result = await _db.Services.Where(s => s.CategoryId == Category.Id).ToListAsync();

            if (result == null)
            {
                return NotFound();
            }

            return View("Service", result);
        }

        public IActionResult Careers()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
