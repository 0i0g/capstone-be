using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_EF.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(AppDbContext db) : base(db)
        {
        }
    }
}