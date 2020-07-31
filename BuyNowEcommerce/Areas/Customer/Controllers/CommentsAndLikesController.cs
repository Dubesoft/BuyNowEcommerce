using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyNowEcommerce.Data;
using BuyNowEcommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CommentsAndLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public CommentsAndLikesController(ApplicationDbContext db,
                              UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PostComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    CommentCount CoummentCountFromDb = await db.CommentCount.Where(c => c.PostId == model.PostId).FirstOrDefaultAsync();

                    if (CoummentCountFromDb == null)
                    {
                        var commentCountModel = new CommentCount
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };

                        db.CommentCount.Add(commentCountModel);
                    }
                    else
                    {
                        CoummentCountFromDb.Count = CoummentCountFromDb.Count + 1;
                    }

                    model.DateCreated = DateTime.UtcNow;
                    model.UserId = UserId;
                    model.Email = Email;
                    db.Comments.Add(model);
                    await db.SaveChangesAsync();

                }
                return Redirect("/Customer/Home/Blog");
            }
            catch (Exception e)
            {

                return Redirect("/Customer/Home/Blog");
            }
        }

        public async Task<IActionResult> AddLike([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    LikeCount LikeCountFromDb = await db.LikeCount.Where(l => l.PostId == model.PostId).FirstOrDefaultAsync();
                    if (LikeCountFromDb == null)
                    {
                        var LikeCountModel = new LikeCount()
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };
                        db.LikeCount.Add(LikeCountModel);
                    }
                    else
                    {
                        LikeCountFromDb.Count = LikeCountFromDb.Count + 1;
                    }
                    await db.SaveChangesAsync();
                }
                return Redirect("/Customer/Home/Blog");
            }
            catch (Exception e)
            {
                return Redirect("/Customer/Home/Blog");
            }
        }

        //public async Task<IActionResult> GetComments(int postId)
        //{
        //    //var postId = Convert.ToInt32(postIds);
        //    List<Comments> comments = new List<Comments>();

        //    //comments = await (from c in _db.Comments
        //    //                  where c.PostId == postId
        //    //                  select c).ToListAsync();
        //    comments = await _db.Comments.Where(c => c.PostId == postId).ToListAsync();
        //    //return Json(new { commentsval = comments });
        //    return Json(comments);
        //}


        public async Task<IActionResult> GetComments(int Id)
        {
            List<Comments> comments = new List<Comments>();

            comments = await db.Comments.Where(p => p.PostId == Id).ToListAsync();
            return PartialView(comments);
            //}
        }

        //public async Task<IActionResult> GetCommentAndLikesCount(int Id)
        //{
        //    List<Comments> comments = new List<Comments>();

        //    CommentsAndLikesViewModels CommentsAndLikesVM = new CommentsAndLikesViewModels()
        //    {
        //        Comments = await db.Comments.Where(p => p.PostId == Id).ToListAsync()
        //    };
        //    return PartialView(CommentsAndLikesVM);
        //    //}
        //}

        public async Task<IActionResult> GetCommentsCount(int Id)
        {
            List<Comments> comments = new List<Comments>();

            comments = await db.Comments.Where(p => p.PostId == Id).ToListAsync();
            return Json(comments.Count());
            //}
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