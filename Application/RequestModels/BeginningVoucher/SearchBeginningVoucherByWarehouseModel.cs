using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class SearchBeginningVoucherByWarehouseModel : PaginationModel
    {
        [Required]
        public Guid WarehouseId { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}