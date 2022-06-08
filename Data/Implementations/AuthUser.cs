using Data.Entities;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Data.Implements;

namespace Data.Entities
{
    public class AuthUser
    {
        public Guid Id { get; set; }

        public ICollection<AuthUserGroup> Groups { get; set; }

        public ICollection<string> Permissions { get; set; }

        public Guid? Warehouse { get; set; }
    }
}
