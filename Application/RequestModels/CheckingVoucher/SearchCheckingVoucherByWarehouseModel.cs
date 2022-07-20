using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class SearchCheckingVoucherByWarehouseModel : PaginationModel
    {
        [Required]
        public Guid WarehouseId { get; set; }
        
        public string Code { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}