using BuyNowEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategory();
        Task<Category> Detail(int? id);
        Task<Category> Create(Category category);
        Task<Category> Edit(Category category);
        Task<Category> Delete(Category category);
    }
}
