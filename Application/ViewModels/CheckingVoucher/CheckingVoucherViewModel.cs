using System;
using System.Collections.Generic;
using Application.ViewModels.BeginningVoucher;

namespace Application.ViewModels.CheckingVoucher
{
    public class CheckingVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public FetchUserViewModel CreateBy { get; set; }

        public FetchWarehouseViewModel Warehouse { get; set; }
        
        public ICollection<CheckingVoucherDetailViewModel> Details { get; set; }
    }
}