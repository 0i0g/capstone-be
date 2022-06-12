using System;
using System.Collections.Generic;

namespace Application.ViewModels.BeginningVoucher
{
    public class BeginningVoucherViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public DateTime ReportingDate { get; set; }

        public string Note { get; set; }

        public ICollection<BeginningVoucherDetailViewModel> Details { get; set; }
    }
}