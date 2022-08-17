using System;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels.User
{
    public class SetUserPermissionModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid GroupId { get; set; }
    }
}