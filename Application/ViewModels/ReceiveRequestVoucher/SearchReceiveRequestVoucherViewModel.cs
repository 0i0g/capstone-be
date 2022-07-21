using System;
using Data.Enums;

namespace Application.ViewModels
{
    public class SearchReceiveRequestVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public EnumStatusRequest Status { get; set; }
        
        public bool? Locked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public FetchCustomerViewModel Customer { get; set; }

        public FetchWarehouseViewModel Warehouse { get; set; }
    }
}