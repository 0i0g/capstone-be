using Data.Entities;

namespace Data_EF.Repositories
{
    public class ReceiveRequestVoucherRepository : Repository<ReceiveRequestVoucher>, IReceiveRequestVoucherRepository
    {
        public ReceiveRequestVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}