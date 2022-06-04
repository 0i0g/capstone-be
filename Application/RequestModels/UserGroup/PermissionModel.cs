using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class PermissionModel
    {
        [Required]
        public string PermissionType { get; set; }

        [Required]
        public int Level { get; set; }
    }
}