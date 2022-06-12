using System;
using Data.Entities;

namespace Application.ViewModels
{
    public class ReceiveRequestVoucherDetailViewModel
    {
        public Guid Id { get; set; }
        
        public int Quantity { get; set; }
        
        public string ProductName { get; set; }
        
        public FetchProductViewModel Product { get; set; }
    }
}