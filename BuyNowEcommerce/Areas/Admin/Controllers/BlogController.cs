using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuyNowEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> logger;
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public BlogController(ILogger<BlogController> logger,
                              ApplicationDbContext db,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            this.logger = logger;
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var post = await db.Post.ToListAsync();
            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            var user = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
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
                    post.UserId = user.Id;
                    post.Picture = p1;
                    post.DateCreated = DateTime.UtcNow;
                }
                db.Post.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView(post);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/Account/ErrorSomethingWentWrong");
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return PartialView(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            var user = await userManager.GetUserAsync(User);
            var postFromDb = await db.Post.Where(p => p.PostId == post.PostId).FirstOrDefaultAsync();

            if (ModelState.IsValid)
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
                    postFromDb.Picture = p1;
                    postFromDb.UserId = user.Id;
                }
                postFromDb.Title = post.Title;
                postFromDb.Description = post.Description;


                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView(post);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return PartialView(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var service = await db.Post.SingleOrDefaultAsync(s => s.PostId == post.PostId);
            var commentCount = await db.CommentCount.SingleOrDefaultAsync(c => c.PostId == post.PostId);
            var comment = await db.Comments.Where(c => c.PostId == post.PostId).ToListAsync();
            var likesCount = await db.LikeCount.SingleOrDefaultAsync(l => l.PostId == post.PostId);

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            db.Post.Remove(service);

            if (comment != null)
            {
                foreach (var item in comment)
                {
                    db.Comments.Remove(item);
                }
            }


            if (commentCount != null)
            {
                db.CommentCount.Remove(commentCount);
            }

            if (likesCount != null)
            {
                db.LikeCount.Remove(likesCount);
            }

            HttpContext.Session.Clear();
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return PartialView(post);
        }
    }
}