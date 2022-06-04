using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class CheckingVoucher : ISafeEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int Inc { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public DateTime ReportingDate { get; set; }

        #region Warehouse

        [Required]
        public Guid WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion
        
        #region Creator

        public Guid CreatorId { get; set; }

        public User Creator { get; set; }

        public string CreatorName { get; set; }

        #endregion
        
        #region Detail

        public ICollection<CheckingVoucherDetail> Details { get; set; }

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