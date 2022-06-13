using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class CreateCustomerModel
    {
        [Required] 
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }
    }
}