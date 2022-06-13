using System;

namespace Application.RequestModels
{
    public class UpdateVoucherPrefixCodeModel
    {
        public Guid Id { get; set; }
        
        public string Prefix { get; set; }
    }
}