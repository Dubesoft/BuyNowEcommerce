using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models;
using BuyNowEcommerce.Models.ViewModels;
using BuyNowEcommerce.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuyNowEcommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CartController(ILogger<CartController> logger,
                              ApplicationDbContext db,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var detailCart = new OrderDetailsCart
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var user = await _userManager.GetUserAsync(User);

            var cart = _db.Cart.Where(c => c.ApplicationUserId == user.Id);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.Services = await _db.Services.FirstOrDefaultAsync(s => s.Id == list.ServicceId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.Services.Price * list.Count);
            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;
            detailCart.OrderHeader.PickupName = user.Name;
            detailCart.OrderHeader.PhoneNumber = user.PhoneNumber;
            detailCart.OrderHeader.PickUpDate = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }

            return View(detailCart);
        }

        [HttpPost]
        public async Task<IActionResult> Summary(string stripeToken)
        {
            var user = await _userManager.GetUserAsync(User);

            var detailCart = new OrderDetailsCart
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.listCart = await _db.Cart.Where(c => c.ApplicationUserId == user.Id).ToListAsync();

            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = user.Id;
            detailCart.OrderHeader.Status = SD.PaymentStatusPending;
            detailCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailCart.OrderHeader.PickUpDate.ToShortDateString() + " " + detailCart.OrderHeader.PickUpTime.ToShortTimeString());


            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(detailCart.OrderHeader);
            await _db.SaveChangesAsync();

            detailCart.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in detailCart.listCart)
            {
                item.Services = await _db.Services.FirstOrDefaultAsync(m => m.Id == item.ServicceId);
                OrderDetails orderDetails = new OrderDetails
                {
                    ServiceId = item.ServicceId,
                    OrderId = detailCart.OrderHeader.Id,
                    Description = item.Services.Description,
                    Name = item.Services.Name,
                    Price = item.Services.Price,
                    Count = item.Count
                };
                detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);

            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotalOriginal;
            }
            detailCart.OrderHeader.CouponCodeDiscount = detailCart.OrderHeader.OrderTotalOriginal - detailCart.OrderHeader.OrderTotal;

            _db.Cart.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32(SD.ssCartCount, 0);
            await _db.SaveChangesAsync();

            return View(detailCart);
        }
    }
}