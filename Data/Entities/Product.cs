using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Product : ISafeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Inc { get; set; }

        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public int? OnHandMin { get; set; }

        public int? OnHandMax { get; set; }

        #region Image

        public Guid? ImageId { get; set; }

        public Attachment Image { get; set; }

        #endregion

        #region Product category

        public ICollection<ProductCategory> ProductCategories { get; set; }

        #endregion

        #region Voucher Detail

        public ICollection<BeginningVoucherDetail> BeginningVoucherDetails { get; set; }

        public ICollection<CheckingVoucherDetail> CheckingVoucherDetails { get; set; }

        public ICollection<DeliveryRequestVoucherDetail> DeliveryRequestVoucherDetails { get; set; }

        public ICollection<FixingVoucherDetail> FixingVoucherDetails { get; set; }
        
        public ICollection<ReceiveRequestVoucherDetail> ReceiveRequestVoucherDetails { get; set; }

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