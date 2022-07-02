using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Application.RequestModels.User
{
    public class UpdateUserModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Gender? Gender { get; set; }
        
        public bool? IsActive { get; set; }

        public Guid? InWarehouseId { get; set; }
    }
}