using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.RequestModels
{
    public class CreateProductModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? OnHandMin { get; set; }

        public int? OnHandMax { get; set; }

        public IFormFile Image { get; set; }

        public Guid? Category { get; set; }
    }
}