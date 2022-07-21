using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateFixingVoucherModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public DateTime? ReportingDate { get; set; }

        public string Description { get; set; }

        public ICollection<FixingVoucherDetailModel> Details { get; set; }
    }
}