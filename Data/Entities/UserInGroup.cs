using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class UserInGroup
    {
        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        public UserGroup Group { get; set; }
    }
}