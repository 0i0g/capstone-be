using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_EF.Repositories
{
    public class CheckingVoucherDetailRepository : Repository<CheckingVoucherDetail>, ICheckingVoucherDetailRepository
    {
        public CheckingVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}
