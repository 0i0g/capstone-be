using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums.Permissions;
using Data.Interfaces;

namespace Application.RequestModels
{
    public class UpdateUserGroupPermissionModel:IGroupPermission
    {
        [Required] 
        public Guid GroupId { get; set; }

        #region Permission

        public PermissionBeginningInventoryVoucher PermissionBeginningInventoryVoucher { get; set; }
        public PermissionCustomer PermissionCustomer { get; set; }
        public PermissionDeliveryRequestVoucher PermissionDeliveryRequestVoucher { get; set; }
        public PermissionDeliveryVoucher PermissionDeliveryVoucher { get; set; }
        public PermissionInventoryCheckingVoucher PermissionInventoryCheckingVoucher { get; set; }
        public PermissionInventoryFixingVoucher PermissionInventoryFixingVoucher { get; set; }
        public PermissionProduct PermissionProduct { get; set; }
        public PermissionReceiveRequestVoucher PermissionReceiveRequestVoucher { get; set; }
        public PermissionReceiveVoucher PermissionReceiveVoucher { get; set; }
        public PermissionTransferRequestVoucher PermissionTransferRequestVoucher { get; set; }
        public PermissionTransferVoucher PermissionTransferVoucher { get; set; }
        public PermissionUser PermissionUser { get; set; }

        #endregion
    }
}