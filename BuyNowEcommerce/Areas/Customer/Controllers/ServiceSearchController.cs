using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ServiceSearchController : Controller
    {
        private readonly ApplicationDbContext db;

        public ServiceSearchController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Service(string value)
        {
            if(value == null)
            {
                return NotFound();
            }

            var result = await db.Category.Where(s => s.Name == value).ToListAsync();

            if (result == null)
            {
                return NotFound();
            }

            return View("Service", result);
        }
    }
}