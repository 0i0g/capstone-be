using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.ViewModels.DeliveryRequestVoucher
{
    public class DeliveryRequestVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}