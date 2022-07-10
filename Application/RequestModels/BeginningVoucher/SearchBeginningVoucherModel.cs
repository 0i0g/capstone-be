using System;

namespace Application.RequestModels
{
    public class SearchBeginningVoucherModel : PaginationModel
    {
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}