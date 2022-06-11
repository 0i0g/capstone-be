using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateCategoryModel
    {
        public Guid Id { get; set; }

        [MinLength(1)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}