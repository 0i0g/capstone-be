using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_EF.Repositories
{
    public class BeginningVoucherRepository : Repository<BeginningVoucher>, IBeginningVoucherRepository
    {
        public BeginningVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}
