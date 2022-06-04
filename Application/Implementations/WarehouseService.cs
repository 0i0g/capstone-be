using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class WarehouseService:BaseService,IWarehouseService
    {
        public WarehouseService(IServiceProvider provider) : base(provider)
        {
        }
    }
}