using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public List<Cart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
