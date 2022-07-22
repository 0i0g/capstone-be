using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class SearchDeliveryVoucherAllWarehouseModel : PaginationModel
    {
        public string Code { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [EnumDataType(typeof(EnumStatusVoucher))]
        public EnumStatusVoucher? Status { get; set; }

        public string Customer { get; set; }
    }
}