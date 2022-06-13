using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateDeliveryRequestVoucherModel
    {
        [Required]
        public DateTime VoucherDate { get; set; }
        
        public string Note { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        
        public ICollection<DeliveryRequestVoucherDetailModel> Details { get; set; }
    }
}