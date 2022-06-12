using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class UpdateDeliveryRequestVoucherStatusModel
    {
        [Required] 
        public Guid Id { get; set; }

        [Required]
        [EnumDataType(typeof(EnumStatusRequest))]
        public EnumStatusRequest Status { get; set; }
    }
}