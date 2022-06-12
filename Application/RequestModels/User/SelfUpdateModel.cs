using System;
using Data.Enums;

namespace Application.RequestModels.User
{
    public class SelfUpdateModel
    {
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Gender? Gender { get; set; }
    }
}