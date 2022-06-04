using System;

namespace Data.Enums.Permissions
{
    [Flags]
    public enum PermissionInventoryCheckingVoucher
    {
        None = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8
    }
}