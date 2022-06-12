using Data.Entities;

namespace Data_EF.Repositories
{
    public class DeliveryRequestVoucherDetailRepository : Repository<DeliveryRequestVoucherDetail>,
        IDeliveryRequestVoucherDetailRepository
    {
        public DeliveryRequestVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}