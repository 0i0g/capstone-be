using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class BeginningVoucherService: BaseService, IBeginningVoucherService
    {
        public BeginningVoucherService(IServiceProvider provider) : base(provider)
        {
        }
    }
}