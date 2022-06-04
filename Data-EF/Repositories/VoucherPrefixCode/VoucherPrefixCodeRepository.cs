using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_EF.Repositories
{
    public class VoucherPrefixCodeRepository : Repository<VoucherPrefixCode>, IVoucherPrefixCodeRepository
    {
        public VoucherPrefixCodeRepository(AppDbContext db) : base(db)
        {
        }
    }
}