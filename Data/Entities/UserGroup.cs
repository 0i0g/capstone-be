using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class UserGroup : ISafeEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #region Users in group

        public ICollection<UserInGroup> UserInGroups { get; set; }

        #endregion

        #region Permissions

        public ICollection<Permission> Permissions { get; set; }

        #endregion

        #region In Warehouse

        public Guid InWarehouseId { get; set; }

        public Warehouse InWarehouse { get; set; }

        #endregion

        // Safe entity
        [Required] public DateTime CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedBy { get; set; }

        [Required] public bool? IsDeleted { get; set; }
    }
}