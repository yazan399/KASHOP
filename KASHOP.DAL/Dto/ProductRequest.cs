using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class ProductRequest
    {
        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public IFormFile MainImage { get; set; }
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal Discount { get; set; }
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
        [Required, MinLength(1)]
        public List<ProductTranslationRequest> Translations { get; set; }
    }
}
