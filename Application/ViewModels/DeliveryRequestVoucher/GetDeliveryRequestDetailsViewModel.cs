using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.DeliveryRequestVoucher
{
    public class GetDeliveryRequestDetailsViewModel
    {
        public Guid Id { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }  
        
        [Range(0, int.MaxValue)]
        public int TotalQuantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}