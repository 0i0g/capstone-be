using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels
{
    public class UpdateDocumentTypeModel
    {
        [Required]
        public int Id { get; set; }
        
        public string Format { get; set; }

        public int? Length { get; set; }
    }
}