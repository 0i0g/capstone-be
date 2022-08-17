using System;
using System.Collections.Generic;

namespace Application.ViewModels.Product
{
    public class SumProductViewModel
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}