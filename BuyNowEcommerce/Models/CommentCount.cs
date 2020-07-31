﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models
{
    public class CommentCount
    {
        public CommentCount()
        {
            Count = 1;
        }
        public int CommentCountId { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        public int Count { get; set; }
    }
}
