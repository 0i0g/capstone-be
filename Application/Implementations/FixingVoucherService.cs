using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class FixingVoucherService : BaseService, IFixingVoucherService
    {
        public FixingVoucherService(IServiceProvider provider) : base(provider)
        {
        }
    }
}