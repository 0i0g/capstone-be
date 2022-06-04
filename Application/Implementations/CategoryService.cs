using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class CategoryService: BaseService, ICategoryService
    {
        public CategoryService(IServiceProvider provider) : base(provider)
        {
        }
    }
}