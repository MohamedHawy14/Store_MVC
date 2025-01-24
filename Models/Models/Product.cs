using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name = "Price List")]
        [Range(1,1000)]
        public double PriceList { get; set; }

        [Required]
        [Display(Name = "Price1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price For50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price For100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

      

        #region Many2One-Category
        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        [ValidateNever]
        public Category? Category { get; set; }
        #endregion

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }




    }
}
