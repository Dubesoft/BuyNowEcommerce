using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models.ViewModels
{
    public class ServiceVm
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public Services Services { get; set; }
        public Cart Cart { get; set; }
    }
}
