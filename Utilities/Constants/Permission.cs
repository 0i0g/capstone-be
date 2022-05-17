using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Constants
{
    public class Permission
    {
        private static readonly string[] PermissionTypes = { "CREATE", "VIEW", "UPDATE", "DELETE" };

        private static readonly string[] PermissionEntities = { "User" };

        private static readonly string[] PermissionExtras = { "CAN.VIEW.PRODUCT.DASHBOARD" };

        public static bool Validate(string value)
        {
            if (PermissionExtras.Contains(value)) return true;
            var parts = value.Split('.');
            if (parts.Length != 2) return false;
            if (PermissionTypes.Contains(parts[0]) || PermissionEntities.Contains(parts[1])) return true;
            return false;
        }

    }
}
