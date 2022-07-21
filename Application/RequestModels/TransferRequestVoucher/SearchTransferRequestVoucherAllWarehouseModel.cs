using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class SearchTransferRequestVoucherAllWarehouseModel : PaginationModel
    {
        public string Code { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [EnumDataType(typeof(EnumStatusRequest))]
        public EnumStatusRequest? Status { get; set; }
    }
}