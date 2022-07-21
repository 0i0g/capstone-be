using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateBeginningVoucherModel
    {
        [Required]
        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        [Required]
        public ICollection<BeginningDetailModel> Details { get; set; }
    }
}