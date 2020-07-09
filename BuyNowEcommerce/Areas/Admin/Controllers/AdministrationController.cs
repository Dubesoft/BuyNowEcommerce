using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BuyNowEcommerce.Areas.Admin.Controllers
{
    public class AdministrationController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        
        {
            return View();
        }
    }
}