using System;
using Application.ViewModels.Warehouse;
using Data.Enums;

namespace Application.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        // public string Password { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public bool? IsActive { get; set; }
        
        public FetchWarehouseViewModel InWarehouse { get; set; }
    }
}