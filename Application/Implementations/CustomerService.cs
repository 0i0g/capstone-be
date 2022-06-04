using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class CustomerService: BaseService, ICustomerService
    {
        public CustomerService(IServiceProvider provider) : base(provider)
        {
        }
    }
}