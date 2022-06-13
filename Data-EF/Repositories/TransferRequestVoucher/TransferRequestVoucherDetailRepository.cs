using Data.Entities;

namespace Data_EF.Repositories
{
    public class TransferRequestVoucherDetailRepository : Repository<TransferRequestVoucherDetail> , ITransferRequestVoucherDetailRepository
    {
        public TransferRequestVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}