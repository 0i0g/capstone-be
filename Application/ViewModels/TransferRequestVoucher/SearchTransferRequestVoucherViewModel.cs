using System;
using Data.Enums;

namespace Application.ViewModels.TransferRequestVoucher
{
    public class SearchTransferRequestVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public EnumStatusRequest Status { get; set; }
        
        public bool? Locked { get; set; }

        public DateTime? CreatedAt { get; set; }
        
        public FetchWarehouseViewModel InboundWarehouse { get; set; }
        
        public FetchWarehouseViewModel OutboundWarehouse { get; set; }
    }
}