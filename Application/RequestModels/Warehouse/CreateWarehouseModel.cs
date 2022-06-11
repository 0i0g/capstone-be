using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateWarehouseModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required] 
        public string Address { get; set; }
    }
}