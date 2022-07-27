using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateReceiveVoucherModel
    {
        public Guid? RequestId { get; set; }
        
        [Required]
        public DateTime ReportingDate { get; set; }
        
        public string Description { get; set; }
        
        public Guid? CustomerId { get; set; }
        
        [Required]
        public ICollection<ReceiveVoucherDetailModel> Details { get; set; }
    }
}