using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Permission
    {
        [Required]
        public string Type { get; set; }

        #region UserGroup

        [Required]
        public Guid UserGroupId { get; set; }

        public UserGroup UserGroup { get; set; }

        #endregion
    }
}