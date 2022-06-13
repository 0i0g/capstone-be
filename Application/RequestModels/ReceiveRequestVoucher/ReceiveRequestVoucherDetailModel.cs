using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class ReceiveRequestVoucherDetailModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public Guid ProductId { get; set; }
    }
}