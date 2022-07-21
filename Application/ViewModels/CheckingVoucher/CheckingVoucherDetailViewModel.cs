using System;

namespace Application.ViewModels.CheckingVoucher
{
    public class CheckingVoucherDetailViewModel
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public int RealQuantity { get; set; }

        public string ProductName { get; set; }

        public FetchProductViewModel Product { get; set; }
    }
}