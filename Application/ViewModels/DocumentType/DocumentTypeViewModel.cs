using System;
using System.Collections.Generic;
using Data.Enums;

namespace Application.ViewModels
{
    public class DocumentTypeViewModel
    {
        public int Id { get; set; }

        public EnumVoucherType Type { get; set; }

        public string Name { get; set; }

        public string Format { get; set; }

        public int Length { get; set; }
    }
}