using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities
{
    public class AuthToken
    {
        public Guid Id { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        #region User

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; } 

        #endregion
    }
}
