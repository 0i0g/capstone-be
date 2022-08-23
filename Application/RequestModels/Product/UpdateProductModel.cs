using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Application.ViewModels.Category;
using Microsoft.AspNetCore.Http;

namespace Application.RequestModels
{
    public class UpdateProductModel
    {
        public Guid Id { get; set; }

        [MinLength(1)]
        public string Name { get; set; }
        
        
        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public int? OnHandMin { get; set; }

        public int? OnHandMax { get; set; }

        public Guid? Category { get; set; }
        
        public IFormFile Image { get; set; }
    }
}