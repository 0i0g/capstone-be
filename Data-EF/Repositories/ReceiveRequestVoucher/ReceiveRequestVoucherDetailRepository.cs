using Data.Entities;

namespace Data_EF.Repositories
{
    public class ReceiveRequestVoucherDetailRepository : Repository<ReceiveRequestVoucherDetail>,
        IReceiveRequestVoucherDetailRepository
    {
        public ReceiveRequestVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}