using System;

namespace Application.ViewModels.Category
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ProductsNum { get; set; }
    }
}