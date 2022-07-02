using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Category : ISafeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        #region Product category

        public ICollection<ProductCategory> ProductCategories { get; set; }

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