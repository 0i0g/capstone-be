using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.RequestModels
{
    public class UpdateUserGroupModel
    {
        [Required]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<string> Permissions { get; set; } = new List<string>();
    }
}