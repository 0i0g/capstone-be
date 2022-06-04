using Data_EF.Repositories;
using System.Threading.Tasks;

namespace Data_EF
{
    public interface IUnitOfWork
    {
        #region Repositories

        public IAttachmentRepository Attachment { get; }

        public IAuthTokenRepository AuthToken { get; }

        public IBeginningVoucherRepository BeginningVoucher { get; }

        public IBeginningVoucherDetailRepository BeginningVoucherDetail { get; }

        public ICategoryRepository Category { get; }
        
        public ICheckingVoucherRepository CheckingVoucher { get; }
        
        public ICheckingVoucherDetailRepository CheckingVoucherDetail { get; }
        
        public ICustomerRepository Customer { get; }
        
        public IProductRepository Product { get; }

        public IUserRepository User { get; }

        public IUserGroupRepository UserGroup { get; }

        public IVoucherPrefixCodeRepository VoucherPrefixCode { get; }

        public IWarehouseRepository Warehouse { get; }

        #endregion

        #region DB method

        Task<int> SaveChanges();

        #endregion
    }
}