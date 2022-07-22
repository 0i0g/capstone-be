using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class UpdateDeliveryVoucherModel
    {
        [Required]
        public Guid Id { get; set; }

        public DateTime? ReportingDate { get; set; }
        
        public string Description { get; set; }

        public Guid? CustomerId { get; set; }

        public EnumStatusVoucher? Status { get; set; }

        public ICollection<DeliveryVoucherDetailModel> Details { get; set; }
    }
}