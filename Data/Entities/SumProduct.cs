using System;

namespace Data.Entities
{
    public class SumProduct
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public Guid WarehouseId { get; set; }

        public int Quantity { get; set; }
    }
}