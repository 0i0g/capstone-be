using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Implements;

namespace Application.ViewModels
{
    public class AuthViewModel
    {
        public Guid UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public AuthUserViewModel User { get; set; }
    }

    public class AuthUserViewModel
    {
        public string Avatar { get; set; }

        public string Name { get; set; }

        public ICollection<AuthUserGroup> Groups { get; set; }
    }
}
