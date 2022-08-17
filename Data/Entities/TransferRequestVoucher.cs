using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Data.Entities
{
    public class TransferRequestVoucher:ISafeEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; }

        [Required]
        public DateTime ReportingDate { get; set; }

        public string Description { get; set; }

        [Required]
        public EnumStatusRequest Status { get; set; }
        
        [Required]
        public bool? Locked { get; set; }

        #region Inbound Warehouse

        [Required]
        public Guid InboundWarehouseId { get; set; }

        public Warehouse InboundWarehouse { get; set; }

        #endregion 
        
        #region Outbound Warehouse

        [Required]
        public Guid OutboundWarehouseId { get; set; }

        public Warehouse OutboundWarehouse { get; set; }

        #endregion
        
        #region Detail

        public ICollection<TransferRequestVoucherDetail> Details { get; set; }

        #endregion

        #region Delivery Man

        public Guid? DeliveryManId { get; set; }

        public User DeliveryMan { get; set; }

        #endregion

        #region Recipient

        public Guid? RecipientId { get; set; }

        public User Recipient { get; set; }

        #endregion

        #region TransferVoucher

        public ICollection<TransferVoucher> TransferVouchers { get; set; }

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