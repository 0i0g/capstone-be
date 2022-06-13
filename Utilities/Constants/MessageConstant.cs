using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Constants
{
    public static class MessageConstant
    {
        // @formatter:off
        /* Audit log */
        public static string AuditLogin => "[{0}] logged in at {1}";
        
        
        /* Authentication */
        public static ResponseMessage AccountNotFound => new() { Code = "1001", Value = "Username does not exist" };
        public static ResponseMessage IncorrectPassword => new() { Code = "1002", Value = "Password does not match with the username" };
        public static ResponseMessage UserDeleted => new() { Code = "1003", Value = "Your account is deleted" };
        public static ResponseMessage UserNotConfirmed => new() { Code = "1004", Value = "Your account is not activated" };
        public static ResponseMessage UserNotInAnyGroup => new() { Code = "1004", Value = "Your account was not assigned to any warehouse" };
        public static ResponseMessage UserBanned => new() { Code = "1005", Value = "Your account is banned" };
        public static ResponseMessage RequiredWarehouse => new() { Code = "1005", Value = "You must be in warehouse to perform this action" };
        public static ResponseMessage AccountNotInAnyWarehouse => new() { Code = "1006", Value = "Your account is not in any BU" };

        /* Authorization */
        public static ResponseMessage RolePermissionForbidden => new() { Code = "2001", Value = "{0}" };
        
        /* BeginningVoucher */
        public static ResponseMessage BeginningVoucherNotFound => new() { Code = "4001", Value = "Beginning voucher does not exist or has been deleted" };
        public static ResponseMessage DuplicateBeginningVoucherDetailsProduct => new() { Code = "4001", Value = "Product name in beginning voucher detail has been duplicated" };
        public static readonly ResponseMessage ProductsInRangeNotFound = new() { Code = "4001", Value = "Products do not exist or have been deleted: {0}" };
        public static readonly ResponseMessage ProductNotFound = new() { Code = "4001", Value = "Product do not exist or have been deleted" };
        public static readonly ResponseMessage BeginningVoucherDetailNotFound = new() { Code = "4001", Value = "Beginning voucher detail does not exist or has been deleted" };
        public static readonly ResponseMessage BeginningVoucherDetailEmpty = new() { Code = "4001", Value = "Beginning voucher detail empty" };
        /* Category */
        public static ResponseMessage CategoryNameExisted => new() { Code = "4001", Value = "Category name already exists" };
        public static ResponseMessage CategoryNotFound => new() { Code = "4001", Value = "Category does not exist" };

        /* Customer */
        public static ResponseMessage CustomerNameExisted => new() { Code = "4001", Value = "Customer name already exists" };
        public static ResponseMessage CustomerNotFound => new() { Code = "4001", Value = "Customer does not exist" };
        
        /* HTTP */
        public static ResponseMessage InternalServerError => new() { Code = "5000", Value = "InternalServerError" };
        public static ResponseMessage InvalidParams => new() { Code = "5003", Value = "Invalid params" };
        public static ResponseMessage InvalidEnumAction => new() { Code = "5003", Value = "Invalid action" };
        public static ResponseMessage OrderByInvalid => new() { Code = "3008", Value = "Order By must be one of: {0}" };
        
        /* Product */
        public static ResponseMessage ProductNameExisted => new() { Code = "4001", Value = "Product name already exists" };
        public static ResponseMessage ProductNotFound => new() { Code = "4001", Value = "Product does not exist" };

        /* UserGroup */
        public static ResponseMessage UserGroupNameExisted => new() { Code = "4001", Value = "Group name already exists" };
        public static ResponseMessage UserGroupNotFound => new() { Code = "4001", Value = "Group does not exist or has been deleted" };
        public static ResponseMessage DuplicateUserGroup => new() { Code = "4001", Value = "User already belongs to this group" };
        public static ResponseMessage CannotUpdateDefaultUserGroup => new() { Code = "4001", Value = "Cannot update default group" };
        public static ResponseMessage CannotRemoveUserGroupContainUser => new() { Code = "4001", Value = "Remove {0} users in this group before delete" };

        /* User */
        public static ResponseMessage ProfileNotFound => new() { Code = "1004", Value = "User does not exist or has been deleted" };
        public static ResponseMessage UserNotFound => new() { Code = "1005", Value = "User does not exist or has been deleted" };
        public static ResponseMessage UserUsernameExisted => new() { Code = "1005", Value = "Username existed" };
        public static ResponseMessage UserEmailExisted => new() { Code = "1005", Value = "Email existed" };
      
        /* ReceiveRequestVoucher */
        public static ResponseMessage ReceiveRequestVoucherNotFound => new() { Code = "4001", Value = "Receive Request Voucher does not exist or has been deleted" };
        public static ResponseMessage DuplicateProductReceiveRequestVoucherDetail => new() { Code = "4001", Value = "Duplicate product in Receive Request Voucher Detail" };
        public static ResponseMessage ReceiveRequestVoucherDetailNotFound => new() { Code = "4001", Value = "Receive Request Voucher Detail does not exist or has been deleted" };
        public static ResponseMessage ForbiddenToUpdateReceiveRequestVoucher  => new() { Code = "4001", Value = "Forbidden to update receive request voucher" };
         
         /* DeliveryRequestVoucher */
        public static ResponseMessage DeliveryRequestVoucherNotFound => new() { Code = "4001", Value = "Delivery Request Voucher does not exist or has been deleted" };
        public static ResponseMessage DuplicateProductDeliveryRequestVoucherDetail => new() { Code = "4001", Value = "Duplicate product in Delivery Request Voucher Detail" };
        public static ResponseMessage ForbiddenToUpdateDeliveryRequestVoucher  => new() { Code = "4001", Value = "Forbidden to update delivery request voucher" };
        public static ResponseMessage DeliveryRequestVoucherDetailNotFound => new() { Code = "4001", Value = "Delivery Request Voucher Detail does not exist or has been deleted" };

        /* Product */
        public static ResponseMessage ProductsNotFound => new() { Code = "4001", Value = "Product with id: {0} do not exist have been deleted" };
        public static ResponseMessage ProductNotFound => new() { Code = "4001", Value = "Product does not exist or has been deleted" };
        
        /* Customer */        
        public static ResponseMessage CustomerNotFound => new() { Code = "4001", Value = "Customer does not exist or has been deleted" };

        /* VoucherPrefixCode */
        public static ResponseMessage VoucherPrefixCodeNotFound => new() { Code = "4001", Value = "Voucher prefix code does not exist or has been deleted" };

        
        /* Warehouse */
        public static ResponseMessage WarehouseNameExisted => new() { Code = "4001", Value = "Warehouse name already exists" };
        public static ResponseMessage WarehouseNotFound => new() { Code = "4001", Value = "Warehouse does not exist" };
        // @formatter:on
    }

    public class ResponseMessage
    {
        public string Code { get; set; }

        public string Value { get; set; }
    }

    public static class ResponseMessageExtension
    {
        public static ResponseMessage WithValues(this ResponseMessage message, params object[] values)
        {
            message.Value = string.Format(message.Value, values);
            return message;
        }
        
        public static ResponseMessage Concat(this ResponseMessage message, ResponseMessage secondMessage)
        {
            var separate = '~';
            message.Value = message.Value + separate + secondMessage.Value;
            
            return message;
        }
    }
}