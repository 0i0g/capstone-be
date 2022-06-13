using System;

namespace Application.RequestModels
{
    public class SearchBeginningVoucherModel : PaginationModel
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}