using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Enums;
using Data.Enums.Permissions;
using Data.Interfaces;

namespace Data.Entities
{
    public class UserGroup : ISafeEntity, IGroupPermission
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public EnumUserGroupType Type { get; set; }

        [Required]
        public bool? IsDefault { get; set; }

        #region Users in group

        public ICollection<UserInGroup> UserInGroups { get; set; }

        #endregion

        #region In Warehouse
        
        public Guid? InWarehouseId { get; set; }

        public Warehouse InWarehouse { get; set; }

        #endregion

        #region Safe entity

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

        #region Permission

        [Required]
        public PermissionBeginningInventoryVoucher PermissionBeginningInventoryVoucher { get; set; }

        [Required]
        public PermissionCustomer PermissionCustomer { get; set; }

        [Required]
        public PermissionDeliveryRequestVoucher PermissionDeliveryRequestVoucher { get; set; }

        [Required]
        public PermissionDeliveryVoucher PermissionDeliveryVoucher { get; set; }

        [Required]
        public PermissionInventoryCheckingVoucher PermissionInventoryCheckingVoucher { get; set; }

        [Required]
        public PermissionInventoryFixingVoucher PermissionInventoryFixingVoucher { get; set; }

        [Required]
        public PermissionProduct PermissionProduct { get; set; }

        [Required]
        public PermissionReceiveRequestVoucher PermissionReceiveRequestVoucher { get; set; }

        [Required]
        public PermissionReceiveVoucher PermissionReceiveVoucher { get; set; }

        [Required]
        public PermissionTransferRequestVoucher PermissionTransferRequestVoucher { get; set; }

        [Required]
        public PermissionTransferVoucher PermissionTransferVoucher { get; set; }

        [Required]
        public PermissionUser PermissionUser { get; set; }

        #endregion
    }
}