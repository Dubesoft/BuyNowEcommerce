using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ServicceId { get; set; }
        [NotMapped]
        [ForeignKey("ServicceId")]
        public virtual Services Services { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to {1}")]
        public int Count { get; set; }
    }
}
