using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class ProductService:BaseService,IProductService
    {
        public ProductService(IServiceProvider provider) : base(provider)
        {
        }
    }
}