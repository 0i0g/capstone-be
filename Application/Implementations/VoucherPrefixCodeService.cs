using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class VoucherPrefixCodeService: BaseService, IVoucherPrefixCodeService
    {
        public VoucherPrefixCodeService(IServiceProvider provider) : base(provider)
        {
        }
    }
}