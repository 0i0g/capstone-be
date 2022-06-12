using System;
using System.Collections.Generic;
using Application.ViewModels.Category;

namespace Application.ViewModels.Product
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        
        
        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public int? OnHandMin { get; set; }

        public int? OnHandMax { get; set; }

        public ICollection<FetchCategoryViewModel> Categories { get; set; }
    }
}