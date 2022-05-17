using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Constants
{
    public static class MessageConstant
    {
        /* Authentication */
        public static readonly ResponseMessage UserNotFound = new() { Code = "1001", Value = "Username does not exist" };
        public static readonly ResponseMessage IncorrectPassword = new() { Code = "1002", Value = "Password does not match with the username" };
        public static readonly ResponseMessage UserDeleted = new() { Code = "1003", Value = "Your account is deleted" };
        public static readonly ResponseMessage UserNotConfirmed = new() { Code = "1004", Value = "Your account is not activated" };
        public static readonly ResponseMessage UserBanned = new() { Code = "1005", Value = "Your account is banned" };
        public static readonly ResponseMessage UserNotInAnyBu = new() { Code = "1006", Value = "Your account is not in any BU" };

        /* Authorization */
        public static readonly ResponseMessage RolePermissionForbidden = new() { Code = "2001", Value = "{0}" };

        /* HTTP */
        public static readonly ResponseMessage InternalServerError = new() { Code = "5000", Value = "InternalServerError" };
        public static readonly ResponseMessage InvalidParams = new() { Code = "5003", Value = "Invalid params" };
        public static readonly ResponseMessage InvalidEnumAction = new() { Code = "5003", Value = "Invalid action" };

        /* Business Unit */
        public static readonly ResponseMessage BusinessUnitNotFound = new() { Code = "4001", Value = "Business Unit does not exist or has been deleted" };
        public static readonly ResponseMessage ParentBusinessUnitNotFound = new() { Code = "4001", Value = "Parent Business Unit does not exist or has been deleted" };
        public static readonly ResponseMessage ParentBusinessForbidden = new() { Code = "4001", Value = "You cannot access this BU" };
        public static readonly ResponseMessage UserIsNotInBuParams = new() { Code = "4001", Value = "The user selected for {0} is not in BU" };

        /* Item */
        public static readonly ResponseMessage ItemNotFound = new ResponseMessage { Code = "4001", Value = "Item does not exist or has been deleted" };
        public static readonly ResponseMessage ParentItemNotFound = new ResponseMessage { Code = "4001", Value = "Parent Item does not exist or has been deleted" };

        /* User message */
        public static readonly ResponseMessage DuplicateUser = new() { Code = "3001", Value = "User already exists" };
        public static readonly ResponseMessage DuplicateRole = new() { Code = "3002", Value = "Role already exists" };
        public static readonly ResponseMessage DuplicateRoleClaim = new() { Code = "3003", Value = "Role claim already exists" };
        public static readonly ResponseMessage RoleNotFound = new() { Code = "3004", Value = "Role not found" };
        public static readonly ResponseMessage RoleClaimNotFound = new() { Code = "3005", Value = "Role claim not found" };
        public static readonly ResponseMessage DuplicateUserRole = new() { Code = "3006", Value = "User role already exists" };
        public static readonly ResponseMessage DuplicateEmail = new() { Code = "3007", Value = "Email already exists" };
        public static readonly ResponseMessage ManagerNotFound = new() { Code = "3008", Value = "Manager does not exist or has been deleted" };
        public static readonly ResponseMessage OrderByInvalid = new() { Code = "3008", Value = "Order By must be one of: {0}" };
        public static readonly ResponseMessage ApproverNotFound = new() { Code = "3009", Value = "Approver not found" };
        public static readonly ResponseMessage SupervisorNotFound = new() { Code = "3010", Value = "Supervisor not found" };
        public static readonly ResponseMessage GovernmentIdInvalid = new() { Code = "3011", Value = "Government Identification is invalid" };
        public static readonly ResponseMessage InvalidYear = new() { Code = "3012", Value = "Year is invalid" };
        public static readonly ResponseMessage EmployeeNotFound = new() { Code = "3013", Value = "Employee does not exist or has been deleted" };

        /* Product */
        public static readonly ResponseMessage ProductCategoryNotFound = new() { Code = "4001", Value = "Product Category does not exist or has been deleted" };
        public static readonly ResponseMessage ProductNotFound = new() { Code = "4001", Value = "Product does not exist or has been deleted" };
        public static readonly ResponseMessage ProductItemCodeNotFound = new() { Code = "4001", Value = "Product Item Code does not exist or has been deleted" };
        public static readonly ResponseMessage ProductItemCodeExisted = new() { Code = "4001", Value = "Product Item Code already exists" };
        public static readonly ResponseMessage InvalidEffectiveDate = new() { Code = "4001", Value = "Effective Date must be less than Expiration Date" };
        public static readonly ResponseMessage ProductQuantityInvalid = new() { Code = "4001", Value = "MinimumQuantity must be less than MaximumQuantity" };

        /* UnitOfMeasurement */
        public static readonly ResponseMessage UnitOfMeasurementNotFound = new() { Code = "4001", Value = "Unit Of Measurement does not exist or has been deleted" };
        public static readonly ResponseMessage UoMIsUsingIn = new() { Code = "4001", Value = "Cannot delete UOM. Using in:" };

        /* BOM */
        public static readonly ResponseMessage BOMNotFound = new() { Code = "4001", Value = "BOM does not exist or has been deleted" };
        public static readonly ResponseMessage BOMRevisionNotFound = new() { Code = "4001", Value = "Revision does not exist or has been deleted" };
        public static readonly ResponseMessage BOMComponentExisted = new() { Code = "4003", Value = "Component already exists" };
        public static readonly ResponseMessage BOMComponentNotFound = new() { Code = "4003", Value = "Component does not exist or has been deleted" };

        /* Customer */
        public static readonly ResponseMessage CustomerNotFound = new() { Code = "4001", Value = "Customer does not exist or has been deleted" };

        /* Warehouse */
        public static readonly ResponseMessage WarehouseNotFound = new() { Code = "4001", Value = "Warehouse does not exist or has been deleted" };

        /* Inventory Record */
        public static readonly ResponseMessage InventoryRecordNotFound = new() { Code = "4001", Value = "Inventory Record does not exist or has been deleted" };
        public static readonly ResponseMessage WhAddLeaderMissingId = new() { Code = "4001", Value = "Please select a leader to add or update" };
        public static readonly ResponseMessage WhLeaderNotFound = new() { Code = "4001", Value = "Leader does not exist or has been deleted" };
        
        /* AppModule */
        public static readonly ResponseMessage AppModuleNotFound = new() { Code = "4001", Value = "Module does not exist or has been deleted" };
        public static readonly ResponseMessage RegisterModuleConflict= new() { Code = "4001", Value = "You registered this module" };
        public static readonly ResponseMessage RegistrationInActive = new() { Code = "4001", Value = "The module is inactive in your system" };
        public static readonly ResponseMessage RegistrationNotFound = new() { Code = "4001", Value = "You did not register this module" };
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
    }
}
