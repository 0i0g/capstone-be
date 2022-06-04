using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ProductCategory
    {
        [Required]
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public Category Category { get; set; }
    }
}