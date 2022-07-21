using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class UpdateTransferRequestVoucherModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public DateTime? ReportingDate { get; set; }

        public string Description { get; set; }
        
        public EnumStatusRequest? Status { get; set; }
        
        public Guid? InboundWarehouseId { get; set; }

        public Guid? DeliveryManId { get; set; }

        public Guid? RecipientId { get; set; }

        public ICollection<TransferRequestVoucherDetailModel> Details { get; set; }
    }
}