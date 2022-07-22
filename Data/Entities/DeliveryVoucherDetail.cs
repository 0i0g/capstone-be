using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class DeliveryVoucherDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        #region Voucher

        [Required]
        public Guid VoucherId { get; set; }

        public DeliveryVoucher Voucher { get; set; }

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