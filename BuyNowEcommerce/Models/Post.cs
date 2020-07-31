using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Video { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
