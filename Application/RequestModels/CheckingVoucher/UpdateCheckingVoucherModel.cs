using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Entities;

namespace Application.RequestModels
{
    public class UpdateCheckingVoucherModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public DateTime? ReportingDate { get; set; }

        public string Description { get; set; }

        public ICollection<CheckingVoucherDetailModel> Details { get; set; }
    }
}