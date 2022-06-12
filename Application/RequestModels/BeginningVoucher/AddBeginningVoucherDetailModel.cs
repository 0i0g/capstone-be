using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class AddBeginningVoucherDetailModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public CreateBeginningVoucherDetailModel Detail { get; set; }
    }
}