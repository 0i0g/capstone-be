using Data.Entities;

namespace Data_EF.Repositories
{
    public class FixingVoucherRepository : Repository<FixingVoucher>, IFixingVoucherRepository
    {
        public FixingVoucherRepository(AppDbContext db) : base(db)
        {
        }
    }
}