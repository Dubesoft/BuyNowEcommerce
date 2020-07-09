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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
