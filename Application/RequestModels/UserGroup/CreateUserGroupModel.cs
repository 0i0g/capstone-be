using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateUserGroupModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}