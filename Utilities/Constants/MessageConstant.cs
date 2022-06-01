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
        public static readonly ResponseMessage UserNotInAnyGroup = new() { Code = "1004", Value = "Your account was not assigned to any warehouse" };
        public static readonly ResponseMessage UserBanned = new() { Code = "1005", Value = "Your account is banned" };

        /* Authorization */
        public static readonly ResponseMessage RolePermissionForbidden = new() { Code = "2001", Value = "{0}" };

        /* HTTP */
        public static readonly ResponseMessage InternalServerError = new() { Code = "5000", Value = "InternalServerError" };
        public static readonly ResponseMessage InvalidParams = new() { Code = "5003", Value = "Invalid params" };
        public static readonly ResponseMessage InvalidEnumAction = new() { Code = "5003", Value = "Invalid action" };
        public static readonly ResponseMessage OrderByInvalid = new() { Code = "3008", Value = "Order By must be one of: {0}" };

        /* User */
        public static readonly ResponseMessage ProfileNotFound = new() { Code = "1004", Value = "User does not exist or has been deleted" };
     
        /* UserGroup */
        public static readonly ResponseMessage UserGroupNameExisted = new() { Code = "4001", Value = "Group name already exists" };
        public static readonly ResponseMessage UserGroupNotFound = new() { Code = "4001", Value = "Group does not exist or has been deleted" };

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
