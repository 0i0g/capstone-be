using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateUserGroupPermissionModel
    {
        [Required] 
        public Guid Id { get; set; }

        public ICollection<PermissionModel> Permissions { get; set; }
    }
}