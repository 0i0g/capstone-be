using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class SearchDeliveryRequestVoucherModel : PaginationModel
    {
        public string Code { get; set; }

        public DateTime? VoucherDateFrom { get; set; }

        public DateTime? VoucherDateTo { get; set; }

        [EnumDataType(typeof(EnumStatusRequest))]
        public EnumStatusRequest? Status { get; set; }

        public string Customer { get; set; }
    }
}