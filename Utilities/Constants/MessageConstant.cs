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

        /* HTTP */
        public static ResponseMessage InternalServerError => new() { Code = "5000", Value = "InternalServerError" };
        public static ResponseMessage InvalidParams => new() { Code = "5003", Value = "Invalid params" };
        public static ResponseMessage InvalidEnumAction => new() { Code = "5003", Value = "Invalid action" };
        public static ResponseMessage OrderByInvalid => new() { Code = "3008", Value = "Order By must be one of: {0}" };

        /* UserGroup */
        public static ResponseMessage UserGroupNameExisted => new() { Code = "4001", Value = "Group name already exists" };
        public static ResponseMessage UserGroupNotFound => new() { Code = "4001", Value = "Group does not exist or has been deleted" };
        public static ResponseMessage DuplicateUserGroup => new() { Code = "4001", Value = "User already belongs to this group" };
        public static ResponseMessage CannotUpdateDefaultUserGroup => new() { Code = "4001", Value = "Cannot update default group" };
        public static ResponseMessage CannotRemoveUserGroupContainUser => new() { Code = "4001", Value = "Remove {0} users in this group before delete" };

        /* User */
        public static ResponseMessage ProfileNotFound => new() { Code = "1004", Value = "User does not exist or has been deleted" };
        public static ResponseMessage UserNotFound => new() { Code = "1005", Value = "User does not exist or has been deleted" };
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
    }
}