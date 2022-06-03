using Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User : ISafeEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        [Required]
        public bool? Confirmed { get; set; }

        [Required]
        public bool? IsActive { get; set; }

        #region Avatar

        public Guid? AvatarId { get; set; }
        
        public Attachment Avatar { get; set; }

        #endregion

        #region User in group
        
        public ICollection<UserInGroup> UserInGroups { get; set; }

        #endregion
        
        #region UserSetting

        [Required]
        public UserSetting UserSetting { get; set; }

        #endregion
        
        #region Token

        public ICollection<AuthToken> AuthTokens { get; set; }

        #endregion

        #region Safe Entity

        [Required]
        public DateTime CreatedAt { get; set; }
        
        public Guid? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? DeletedAt { get; set; }
        
        public Guid? DeletedBy { get; set; }
        
        [Required]
        public bool? IsDeleted { get; set; }

        #endregion

        #region Test

        public Test Test { get; set; }
        
        #endregion
    }
}
