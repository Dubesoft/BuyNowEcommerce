using BuyNowEcommerce.Data;
using BuyNowEcommerce.Interfaces;
using BuyNowEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext db;
        public CategoryRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            return await db.Category.ToListAsync();
        }

        public async Task<Category> Detail(int? id)
        {
            return await db.Category.FindAsync(id);
        }


        public async Task<Category> Create(Category category)
        {
            var result = await db.Category.AddAsync(category);
            await db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Category> Edit(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var result = db.Update(category);
            await db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Category> Delete(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            var result = db.Category.Remove(category);
            await db.SaveChangesAsync();
            return result.Entity;
        }
    }
}
