using System;

namespace Application.RequestModels
{
    public class SearchBeginningVoucherInWarehouseModel : PaginationModel
    {
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}