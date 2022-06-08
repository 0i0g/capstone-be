using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateUserGroupPermissionModel
    {
        [Required] 
        public Guid GroupId { get; set; }

        public ICollection<string> Permissions { get; set; }
    }
}