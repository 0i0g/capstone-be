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

        public string Description { get; set; }

        public ICollection<BeginningDetailCreateModel> AddDetails { get; set; }
        
        public ICollection<BeginningDetailUpdateModel> UpdateDetails { get; set; }
        
        public ICollection<Guid> DeleteDetails { get; set; }
    }
}