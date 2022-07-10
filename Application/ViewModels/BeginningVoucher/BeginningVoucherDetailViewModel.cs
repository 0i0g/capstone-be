using System;

namespace Application.ViewModels.BeginningVoucher
{
    public class BeginningVoucherDetailViewModel
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public FetchProductViewModel Product { get; set; }
    }
}