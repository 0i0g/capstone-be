using System;
using System.Collections.Generic;
using Data.Enums;

namespace Application.ViewModels
{
    public class UserProfile
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Gender? Gender { get; set; }

        public string Description { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public bool Confirmed { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
