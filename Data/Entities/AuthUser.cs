using Data.Entities;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities
{
    public class AuthUser
    {
        public Guid Id { get; set; }

        public ICollection<string> Roles { get; set; }

        public ICollection<string> Permissions { get; set; }
        
        public Guid? WarehouseId { get; set; }
    }
}
