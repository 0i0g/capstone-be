using System;

namespace Application.ViewModels.TransferVoucher
{
    public class TransferVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int? RequestQuantity { get; set; }
        
        public int VoucherQuantity { get; set; }

        public int? RealQuantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}