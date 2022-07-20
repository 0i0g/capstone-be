using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class FixingVoucherDetailModel
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [EnumDataType(typeof(EnumFixing))]
        public EnumFixing Type { get; set; }
    }
}