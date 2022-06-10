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

        [Required]
        public int Inc { get; set; }

        public string Code { get; set; }

        [Required]
        public DateTime VoucherDate { get; set; }

        public string Note { get; set; }

        [Required]
        public EnumStatusRequest Status { get; set; }
        
        [Required]
        public bool? Locked { get; set; }

        #region Customer

        public Guid? CustomerId { get; set; }

        public Customer Customer { get; set; }
        
        #endregion
        
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

        // TODO add reference people
        
        #region Delivery Man

        

        #endregion

        #region Recipient

        

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