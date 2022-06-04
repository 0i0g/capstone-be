using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class VoucherPrefixCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string VoucherName { get; set; }

        [Required]
        public string Prefix { get; set; }
    }
}