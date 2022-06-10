using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class FixingVoucher: ISafeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Inc { get; set; }

        public string Code { get; set; }

        [Required]
        public DateTime ReportingDate { get; set; }

        public string Note { get; set; }
        
        #region Warehouse

        [Required]
        public Guid WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion

        #region Detail

        public ICollection<FixingVoucherDetail> Details { get; set; }

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