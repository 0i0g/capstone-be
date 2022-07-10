using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class BeginningDetailCreateModel
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}