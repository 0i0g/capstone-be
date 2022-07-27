using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class ReceiveTransferVoucherModel
    {
        [Required]
        public Guid Id { get; set; }

        public ICollection<ReceiveTransferVoucherDetailModel> Details { get; set; }
    }
}