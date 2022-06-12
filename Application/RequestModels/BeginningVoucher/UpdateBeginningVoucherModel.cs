using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateBeginningVoucherModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public DateTime? ReportingDate { get; set; }

        public string Note { get; set; }

        public UpdateBeginningVoucherDetailModel Detail { get; set; }
    }
}