using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Services> Service { get; set; }
    }
}
