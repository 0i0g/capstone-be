using System;

namespace Application.RequestModels
{
    public class SearchFixingVoucherAllWarehouseModel : PaginationModel
    {
        public string Code { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}