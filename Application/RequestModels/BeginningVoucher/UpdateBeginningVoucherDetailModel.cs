using System;

namespace Application.RequestModels
{
    public class UpdateBeginningVoucherDetailModel
    {
        public Guid Id { get; set; }
        
        public int? Quantity { get; set; }

        public Guid? ProductId { get; set; }
    }
}