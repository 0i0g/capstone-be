using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateTransferVoucherModel
    {
        public Guid? RequestId { get; set; }
        
        [Required]
        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid InboundWarehouseId { get; set; }

        public Guid? DeliveryManId { get; set; }

        public Guid? RecipientId { get; set; }

        [Required]
        public ICollection<TransferVoucherDetailModel> Details { get; set; }
    }
}