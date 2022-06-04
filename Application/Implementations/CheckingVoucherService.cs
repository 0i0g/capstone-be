using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class CheckingVoucherService: BaseService, ICheckingVoucherService
    {
        public CheckingVoucherService(IServiceProvider provider) : base(provider)
        {
        }
    }
}