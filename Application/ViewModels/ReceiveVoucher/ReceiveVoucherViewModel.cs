using System;
using System.Collections.Generic;
using Application.ViewModels.DeliveryRequestVoucher;
using Application.ViewModels.DeliveryVoucher;
using Data.Enums;

namespace Application.ViewModels.ReceiveVoucher
{
    public class ReceiveVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public EnumStatusVoucher Status { get; set; }
        
        public bool? Locked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public FetchUserViewModel CreateBy { get; set; }

        public FetchReceiveRequestVoucherViewModel Request { get; set; }

        public FetchCustomerViewModel Customer { get; set; }
        
        public FetchWarehouseViewModel Warehouse { get; set; }
        
        public ICollection<ReceiveVoucherDetailViewModel> Details { get; set; }
    }
}