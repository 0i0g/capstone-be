using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_EF.Repositories
{
    public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(AppDbContext db) : base(db)
        {
        }
    }
}