using System;

namespace Application.ViewModels
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Avatar { get; set; }

        public string PhoneNumber { get; set; }

        public string  Gender { get; set; }

        public Guid? InWarehouseId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
