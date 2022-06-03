using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels.Test
{
    public class CreateTestModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Name { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}