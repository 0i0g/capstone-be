using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Data_EF.Repositories
{
    public class SumProductRepository : Repository<SumProduct>, ISumProductRepository
    {
        public SumProductRepository(AppDbContext db) : base(db)
        {
        }
    }
}