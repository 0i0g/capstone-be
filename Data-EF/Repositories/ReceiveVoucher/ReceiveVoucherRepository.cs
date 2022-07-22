using Data.Entities;

namespace Data_EF.Repositories
{
    public class ReceiveVoucherRepository : Repository<ReceiveVoucher>, IReceiveVoucherRepository
    {
        public ReceiveVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}