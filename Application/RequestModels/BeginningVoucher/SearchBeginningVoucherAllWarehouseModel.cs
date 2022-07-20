using System;

namespace Application.RequestModels
{
    public class SearchBeginningVoucherAllWarehouseModel : PaginationModel
    {
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}