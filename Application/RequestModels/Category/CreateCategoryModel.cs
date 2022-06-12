using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateCategoryModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}