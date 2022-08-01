using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Enums;

namespace Data.Entities
{
    public class DocumentType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required] 
        public EnumVoucherType Type { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Format { get; set; }

        public int Length { get; set; }
    }
}