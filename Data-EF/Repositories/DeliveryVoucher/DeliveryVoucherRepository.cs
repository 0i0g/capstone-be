using Data.Entities;

namespace Data_EF.Repositories
{
    public class DeliveryVoucherRepository : Repository<DeliveryVoucher>, IDeliveryVoucherRepository
    {
        public DeliveryVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}