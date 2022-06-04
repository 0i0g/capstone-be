using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class AuthToken
    {
        [Key]
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