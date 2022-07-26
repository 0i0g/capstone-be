using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class ReceiveTransferVoucherDetailModel
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int RealQuantity { get; set; }
    }
}