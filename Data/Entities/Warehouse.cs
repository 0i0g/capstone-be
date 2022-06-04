using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Warehouse : ISafeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        #region Users

        public ICollection<User> Users { get; set; }

        #endregion

        #region UserGroup

        public ICollection<UserGroup> UserGroups { get; set; }

        #endregion

        #region Beginning inventory

        public ICollection<BeginningVoucher> BeginningVouchers { get; set; }

        #endregion 
        
        #region Checking inventory

        public ICollection<CheckingVoucher> CheckingVouchers { get; set; }

        #endregion

        #region Safe entity

        [Required]
        public DateTime CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedBy { get; set; }

        [Required]
        public bool? IsDeleted { get; set; }

        #endregion
    }
}