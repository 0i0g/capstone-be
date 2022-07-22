using Data.Entities;

namespace Data_EF.Repositories
{
    public class TransferVoucherRepository : Repository<TransferVoucher>, ITransferVoucherRepository
    {
        public TransferVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}