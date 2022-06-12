using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateBeginningVoucherModel
    {
        [Required]
        public DateTime ReportingDate { get; set; }

        public string Note { get; set; }

        public ICollection<CreateBeginningVoucherDetailModel> Details { get; set; }
    }
}