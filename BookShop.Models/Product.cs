using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product Name")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [DisplayName("List Price")]
        [Required]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [DisplayName("Price for 1-50")]
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }

        [DisplayName("Price for 50+")]
        [Required]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [DisplayName("Price for 100+")]
        [Required]
        [Range(1, 1000)]
        public double Price100 { get; set; }
        
        public int CategoryId { get;set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

    }
}
