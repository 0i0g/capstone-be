using Data.Entities;

namespace Data_EF.Repositories
{
    public class TransferRequestVoucherRepository : Repository<TransferRequestVoucher>, ITransferRequestVoucherRepository
    {
        public TransferRequestVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}