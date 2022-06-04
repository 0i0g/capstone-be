using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_EF.Repositories
{
    public class CheckingVoucherRepository : Repository<CheckingVoucher>, ICheckingVoucherRepository
    {
        public CheckingVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}
