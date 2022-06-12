using System;
using System.Collections.Generic;
using Data.Enums;

namespace Application.ViewModels.DeliveryRequestVoucher
{
    public class DeliveryRequestVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime VoucherDate { get; set; }

        public string Note { get; set; }

        public EnumStatusRequest Status { get; set; }
        
        public bool? Locked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public FetchCustomerViewModel Customer { get; set; }
        
        public FetchWarehouseViewModel Warehouse { get; set; }
        
        public ICollection<DeliveryRequestVoucherDetailViewModel> Details { get; set; }
    }
}