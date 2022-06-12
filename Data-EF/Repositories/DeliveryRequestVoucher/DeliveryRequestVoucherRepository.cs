using Data.Entities;

namespace Data_EF.Repositories
{
    public class DeliveryRequestVoucherRepository : Repository<DeliveryRequestVoucher>, IDeliveryRequestVoucherRepository
    {
        public DeliveryRequestVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}