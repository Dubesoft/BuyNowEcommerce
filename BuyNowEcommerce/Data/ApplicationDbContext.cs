using System;
using System.Collections.Generic;
using System.Text;
using BuyNowEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuyNowEcommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<CommentCount> CommentCount { get; set; }
        public DbSet<LikeCount> LikeCount { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
