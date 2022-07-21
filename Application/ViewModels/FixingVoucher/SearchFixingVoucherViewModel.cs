using System;

namespace Application.ViewModels.FixingVoucher
{
    public class SearchFixingVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public FetchWarehouseViewModel Warehouse { get; set; }
    }
}