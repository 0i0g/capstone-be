using System;

namespace Application.ViewModels.BeginningVoucher
{
    public class SearchBeginningVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public FetchWarehouseViewModel Warehouse { get; set; }
    }
}