using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNowEcommerce.Models
{
    public class Services
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public double Price { get; set; }
    }
}
