using Data.Entities;

namespace Data_EF.Repositories
{
    public class FixingVoucherDetailRepository : Repository<FixingVoucherDetail>, IFixingVoucherDetailRepository
    {
        public FixingVoucherDetailRepository(AppDbContext db) : base(db)
        {
        }
    }
}