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

        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public int? OnHandMin { get; set; }

        public int? OnHandMax { get; set; }

        #region Image

        public string Image { get; set; }

        #endregion

        #region Category

        public Guid? CategoryId { get; set; }

        public Category Category { get; set; }

        #endregion

        #region Voucher Detail

        public ICollection<BeginningVoucherDetail> BeginningVoucherDetails { get; set; }

        public ICollection<CheckingVoucherDetail> CheckingVoucherDetails { get; set; }

        public ICollection<DeliveryRequestVoucherDetail> DeliveryRequestVoucherDetails { get; set; }
        
        public ICollection<DeliveryVoucherDetail> DeliveryVoucherDetails { get; set; }

        public ICollection<FixingVoucherDetail> FixingVoucherDetails { get; set; }
        
        public ICollection<ReceiveRequestVoucherDetail> ReceiveRequestVoucherDetails { get; set; }
        
        public ICollection<ReceiveVoucherDetail> ReceiveVoucherDetails { get; set; }
        
        public ICollection<TransferRequestVoucherDetail> TransferRequestVoucherDetails { get; set; }
        
        public ICollection<TransferVoucherDetail> TransferVoucherDetails { get; set; }

        #endregion

        #region Safe entity

        [Required]
        public DateTime CreatedAt { get; set; }

        public Guid? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedById { get; set; }

        public User DeletedBy { get; set; }

        [Required]
        public bool? IsDeleted { get; set; }

        #endregion
    }
}