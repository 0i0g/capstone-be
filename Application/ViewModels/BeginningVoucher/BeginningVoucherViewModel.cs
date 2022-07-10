using System;
using System.Collections.Generic;

namespace Application.ViewModels.BeginningVoucher
{
    public class BeginningVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public FetchWarehouseViewModel Warehouse { get; set; }
        
        public ICollection<BeginningVoucherDetailViewModel> Details { get; set; }
    }
}