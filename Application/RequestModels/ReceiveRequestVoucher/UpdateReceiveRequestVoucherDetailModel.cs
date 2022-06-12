using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateReceiveRequestVoucherDetailModel
    {
        [Required]
        public Guid Id { get; set; }
        
        [Range(1, int.MaxValue)] 
        public int? Quantity { get; set; }

        public Guid? ProductId { get; set; }
    }
}