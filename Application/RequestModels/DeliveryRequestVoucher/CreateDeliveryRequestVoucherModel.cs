using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateDeliveryRequestVoucherModel
    {
        [Required]
        public DateTime ReportingDate { get; set; }
        
        public string Description { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        
        public ICollection<DeliveryRequestVoucherDetailModel> Details { get; set; }
    }
}