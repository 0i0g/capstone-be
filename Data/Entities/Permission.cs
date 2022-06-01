using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums.Permissions;

namespace Data.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }

        public EnumUserPermission UserPermission { get; set; }

        public EnumWarehousePermission WarehousePermission { get; set; }

        public EnumProductPermission ProductPermission { get; set; }

        #region UserGroup

        [Required]
        public Guid GroupId { get; set; }

        public UserGroup Group { get; set; }

        #endregion
    }
}