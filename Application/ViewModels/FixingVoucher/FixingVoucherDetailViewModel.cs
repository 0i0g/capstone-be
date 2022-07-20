using System;
using Data.Enums;

namespace Application.ViewModels.FixingVoucher
{
    public class FixingVoucherDetailViewModel
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public EnumFixing Type { get; set; }

        public string ProductName { get; set; }

        public FetchProductViewModel Product { get; set; }
    }
}