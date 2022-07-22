using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateDeliveryVoucherModel
    {
        public Guid? RequestId { get; set; }
        
        [Required]
        public DateTime ReportingDate { get; set; }
        
        public string Description { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        
        [Required]
        public ICollection<DeliveryVoucherDetailModel> Details { get; set; }
    }
}