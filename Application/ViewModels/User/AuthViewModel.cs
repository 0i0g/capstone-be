using System;
using System.Collections.Generic;

namespace Application.ViewModels
{
    public class AuthViewModel
    {
        public Guid UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class AuthUserViewModel
    {
        public string Avatar { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }
        
        public ICollection<string> Permissions { get; set; }
    }
}
