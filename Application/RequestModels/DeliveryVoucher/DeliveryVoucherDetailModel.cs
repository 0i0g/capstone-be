using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class DeliveryVoucherDetailModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public Guid ProductId { get; set; }
    }
}