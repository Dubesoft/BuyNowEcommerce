using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Interfaces;
using BuyNowEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public  async Task<IActionResult> Index()
        {
            return View(await categoryRepository.GetCategory());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                await categoryRepository.Create(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var category = await categoryRepository.Detail(id);

            if(category == null)
            {
                return NotFound();
            }
            return PartialView(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if(ModelState.IsValid)
            {
                await categoryRepository.Edit(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var category = await categoryRepository.Detail(id);

            if (category == null)
            {
                return NotFound();
            }
            return PartialView(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Category category)
        {
            if(ModelState.IsValid)
            {
                await categoryRepository.Delete(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await categoryRepository.Detail(id);

            if (category == null)
            {
                return NotFound();
            }
            return PartialView(category);
        }
    }
}