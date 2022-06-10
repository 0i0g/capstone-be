using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Enums;

namespace Data.Entities
{
    public class TransferRequestVoucherDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        #region Voucher

        [Required]
        public Guid VoucherId { get; set; }

        public TransferRequestVoucher Voucher { get; set; }

        #endregion

        #region Product

        [Required]
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public string ProductName { get; set; }

        #endregion
    }
}