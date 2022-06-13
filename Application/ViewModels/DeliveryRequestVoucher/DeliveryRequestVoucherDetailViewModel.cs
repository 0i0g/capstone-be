using System;
using System.Collections.Generic;
using Data.Enums;

namespace Application.ViewModels.DeliveryRequestVoucher
{
    public class DeliveryRequestVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int Quantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}