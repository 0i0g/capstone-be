using System;
using System.Collections.Generic;
using Application.ViewModels.TransferRequestVoucher;
using Data.Enums;

namespace Application.ViewModels.TransferVoucher
{
    public class TransferVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public EnumStatusVoucher Status { get; set; }
        
        public bool? Locked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public FetchUserViewModel CreateBy { get; set; }

        public FetchTransferRequestVoucherViewModel Request { get; set; }
        
        public FetchWarehouseViewModel InboundWarehouse { get; set; }
        
        public FetchWarehouseViewModel OutboundWarehouse { get; set; }

        public FetchUserViewModel DeliveryMan { get; set; }

        public FetchUserViewModel Recipient { get; set; }
        
        public ICollection<TransferVoucherDetailViewModel> Details { get; set; }
    }
}