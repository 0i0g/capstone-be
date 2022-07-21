using System;

namespace Application.ViewModels.TransferRequestVoucher
{
    public class TransferRequestVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int Quantity { get; set; }

        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}