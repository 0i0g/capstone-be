using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateCustomerModel
    {
        public Guid Id { get; set; }

        [MinLength(1)]
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }
    }
}