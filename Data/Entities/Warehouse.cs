using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Warehouse
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        #region UserGroup

        public ICollection<UserGroup> UserGroups { get; set; }

        #endregion
    }
}