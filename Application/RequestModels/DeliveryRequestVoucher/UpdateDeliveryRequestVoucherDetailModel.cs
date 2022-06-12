using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.RequestModels
{
    public class UpdateDeliveryRequestVoucherDetailModel
    {
        [Required]
        public Guid Id { get; set; }
        
        [Range(1, int.MaxValue)] 
        public int? Quantity { get; set; }

        public Guid? ProductId { get; set; }
    }
}