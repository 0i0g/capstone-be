using System;

namespace Application.ViewModels.ReceiveVoucher
{
    public class ReceiveVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int? RequestQuantity { get; set; }
        
        public int VoucherQuantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}