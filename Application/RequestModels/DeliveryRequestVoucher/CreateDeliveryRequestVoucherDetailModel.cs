using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateDeliveryRequestVoucherDetailModel
    {
        [Required] 
        [Range(1, int.MaxValue)] 
        public int Quantity { get; set; }

        [Required]
        public Guid VoucherId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
    }
}