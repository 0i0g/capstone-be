using System;

namespace Application.RequestModels
{
    public class CreateBeginningVoucherDetailModel
    {
        public int Quantity { get; set; }

        public Guid ProductId { get; set; }
    }
}