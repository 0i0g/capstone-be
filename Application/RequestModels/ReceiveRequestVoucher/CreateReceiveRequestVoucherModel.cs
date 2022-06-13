using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateReceiveRequestVoucherModel
    {
        [Required]
        public DateTime VoucherDate { get; set; }
        
        public string Note { get; set; }

        public Guid? CustomerId { get; set; }
        
        public ICollection<ReceiveRequestVoucherDetailModel> Details { get; set; }
    }
}