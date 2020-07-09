using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ServiceController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var Service = await _db.Services.Include(s => s.Category).ToListAsync();
            return View(Service);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ServiceVm serviceVm = new ServiceVm()
            {
                CategoryList = await _db.Category.ToListAsync(),
                Services = new Models.Services()
            };
            return PartialView(serviceVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceVm model)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
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
                    model.Services.Picture = p1;
                }

                _db.Services.Add(model.Services);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var service = await _db.Services.FindAsync(id);
            ServiceVm serviceVm = new ServiceVm()
            {
                 CategoryList = await _db.Category.Where(s => s.Id == id).ToListAsync(),
                 Services = await _db.Services.SingleOrDefaultAsync(s => s.Id == id)
            };

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return PartialView(serviceVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceVm model)
        {
            var serviceFromDb = await _db.Services.Where(s => s.Id == model.Services.Id).FirstOrDefaultAsync();

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
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
                    serviceFromDb.Picture = p1;
                }

                serviceFromDb.Name = model.Services.Name;
                serviceFromDb.CategoryId = model.Services.CategoryId;
                serviceFromDb.Description = model.Services.Description;
                serviceFromDb.Price = model.Services.Price;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
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

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return PartialView(serviceVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ServiceVm model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var service = await _db.Services.SingleOrDefaultAsync(s => s.Id == model.Services.Id);

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
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

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return PartialView(serviceVm);
        }
    }
}