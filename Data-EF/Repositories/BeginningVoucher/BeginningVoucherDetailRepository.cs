using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_EF.Repositories
{
    public class BeginningVoucherDetailRepository : Repository<BeginningVoucherDetail>, IBeginningVoucherDetailRepository
    {
        public BeginningVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}
