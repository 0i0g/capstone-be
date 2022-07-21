using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class UpdateReceiveRequestVoucherModel
    {
        [Required] 
        public Guid Id { get; set; }

        public DateTime? ReportingDate { get; set; }
        
        public string Description { get; set; }

        public Guid? CustomerId { get; set; }

        public bool IsCustomerIdNull { get; set; } = false;

        public EnumStatusRequest? Status { get; set; }
        
        public ICollection<ReceiveRequestVoucherDetailModel> Details { get; set; }
    }
}