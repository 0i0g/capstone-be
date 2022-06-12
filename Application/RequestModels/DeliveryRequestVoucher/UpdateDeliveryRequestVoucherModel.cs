using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateDeliveryRequestVoucherModel
    {
        [Required] 
        public Guid Id { get; set; }

        public DateTime? VoucherDate { get; set; }
        
        public string Note { get; set; }

        public Guid? CustomerId { get; set; }
    }
}