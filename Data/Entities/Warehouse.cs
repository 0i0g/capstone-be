using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        #region Vouchers

        public ICollection<BeginningVoucher> BeginningVouchers { get; set; }

        public ICollection<CheckingVoucher> CheckingVouchers { get; set; }

        public ICollection<DeliveryRequestVoucher> DeliveryRequestVouchers { get; set; }
        
        public ICollection<DeliveryVoucher> DeliveryVouchers { get; set; }

        public ICollection<FixingVoucher> FixingVouchers { get; set; }

        public ICollection<ReceiveRequestVoucher> ReceiveRequestVouchers { get; set; }
        
        public ICollection<ReceiveVoucher> ReceiveVouchers { get; set; }
        
        public ICollection<TransferRequestVoucher> InboundTransferRequestVouchers { get; set; }
        
        public ICollection<TransferRequestVoucher> OutboundTransferRequestVouchers { get; set; }
        
        public ICollection<TransferVoucher> InboundTransferVouchers { get; set; }
        
        public ICollection<TransferVoucher> OutboundTransferVouchers { get; set; }

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