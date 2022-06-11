using System.Collections.Generic;
using System.Linq;

namespace Application.Utilities

{
    public class PermissionManager
    {
        public static IEnumerable<string> WarehousePermissions =>
            new List<string>
            {
                "Permission.User.Create",
                "Permission.User.Read",
                "Permission.User.Update",
                "Permission.User.Delete",

                "Permission.BeginningInventoryVoucher.Create",
                "Permission.BeginningInventoryVoucher.Read",
                "Permission.BeginningInventoryVoucher.Update",
                "Permission.BeginningInventoryVoucher.Delete",

                "Permission.Customer.Create",
                "Permission.Customer.Read",
                "Permission.Customer.Update",
                "Permission.Customer.Delete",

                "Permission.DeliveryRequestVoucher.Create",
                "Permission.DeliveryRequestVoucher.Read",
                "Permission.DeliveryRequestVoucher.Update",
                "Permission.DeliveryRequestVoucher.Delete",

                "Permission.DeliveryVoucher.Create",
                "Permission.DeliveryVoucher.Read",
                "Permission.DeliveryVoucher.Update",
                "Permission.DeliveryVoucher.Delete",

                "Permission.InventoryCheckingVoucher.Create",
                "Permission.InventoryCheckingVoucher.Read",
                "Permission.InventoryCheckingVoucher.Update",
                "Permission.InventoryCheckingVoucher.Delete",

                "Permission.InventoryFixingVoucher.Create",
                "Permission.InventoryFixingVoucher.Read",
                "Permission.InventoryFixingVoucher.Update",
                "Permission.InventoryFixingVoucher.Delete",

                "Permission.Product.Create",
                "Permission.Product.Read",
                "Permission.Product.Update",
                "Permission.Product.Delete",

                "Permission.ReceiveRequestVoucher.Create",
                "Permission.ReceiveRequestVoucher.Read",
                "Permission.ReceiveRequestVoucher.Update",
                "Permission.ReceiveRequestVoucher.Delete",

                "Permission.ReceiveVoucher.Create",
                "Permission.ReceiveVoucher.Read",
                "Permission.ReceiveVoucher.Update",
                "Permission.ReceiveVoucher.Delete",

                "Permission.TransferRequestVoucher.Create",
                "Permission.TransferRequestVoucher.Read",
                "Permission.TransferRequestVoucher.Update",
                "Permission.TransferRequestVoucher.Delete",

                "Permission.TransferVoucher.Create",
                "Permission.TransferVoucher.Read",
                "Permission.TransferVoucher.Update",
                "Permission.TransferVoucher.Delete",
                
                "Permission.Warehouse.Create",
                "Permission.Warehouse.Read",
                "Permission.Warehouse.Update",
                "Permission.Warehouse.Delete",
            };

        public static IEnumerable<string> SystemPermissions =>
            new List<string>
            {
                "Permission.System.Master",
                "Permission.System.Admin",
                "Permission.System.BuManager"
            };

        public static ICollection<string> GetValidWarehousePermission(IEnumerable<string> values)
        {
            return values.Intersect(WarehousePermissions).ToList();
        }

        public static ICollection<string> GetValidSystemPermission(IEnumerable<string> values)
        {
            return values.Intersect(SystemPermissions).ToList();
        }
    }
}