using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.DeliveryVoucher
{
    public class DeliveryVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int? RequestQuantity { get; set; }
        
        public int VoucherQuantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}