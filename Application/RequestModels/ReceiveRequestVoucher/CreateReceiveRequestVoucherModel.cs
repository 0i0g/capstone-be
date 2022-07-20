using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateReceiveRequestVoucherModel
    {
        [Required]
        public DateTime ReportingDate { get; set; }
        
        public string Description { get; set; }

        public Guid? CustomerId { get; set; }
        
        public ICollection<ReceiveRequestVoucherDetailModel> Details { get; set; }
    }
}