using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuyNowEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdministrationController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(ApplicationDbContext db,
                                        RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        ILogger<AdministrationController> logger)
        {
            this.db = db;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ErrorOccured()
        {
            return View();
        }
    }
}