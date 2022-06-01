using System;
using System.Collections.Generic;
using Application.ViewModels.Permission;

namespace Application.ViewModels.UserGroup
{
    public class UserGroupViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<PermissionViewModel> Permissions { get; set; }
    }
}