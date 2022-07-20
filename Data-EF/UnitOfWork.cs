using Data_EF.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_EF
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Repositories

        private IAttachmentRepository _attachment;

        private IAuditLogRepository _auditLog;

        private IAuthTokenRepository _authToken;

        private IBeginningVoucherRepository _beginningVoucher;

        private IBeginningVoucherDetailRepository _beginningVoucherDetail;

        private ICategoryRepository _category;

        private ICheckingVoucherRepository _checkingVoucher;

        private ICheckingVoucherDetailRepository _checkingVoucherDetail;

        private ICustomerRepository _customer;

        private IPermissionRepository _permission;

        private IProductRepository _product;

        private IUserRepository _user;

        private IUserGroupRepository _userGroup;

        private IVoucherPrefixCodeRepository _voucherPrefixCode;

        private IWarehouseRepository _warehouse;

        private ITestRepository _testSetting;

        private IReceiveRequestVoucherRepository _receiveRequestVoucher;

        private IReceiveRequestVoucherDetailRepository _receiveRequestVoucherDetail;

        private IDeliveryRequestVoucherRepository _deliveryRequestVoucher;

        private IDeliveryRequestVoucherDetailRepository _deliveryRequestVoucherDetail;

        private IFixingVoucherRepository _fixingVoucher;

        #endregion

        public UnitOfWork(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Get Repositories

        public IAttachmentRepository Attachment
        {
            get { return _attachment ??= new AttachmentRepository(_db); }
        }

        public IAuditLogRepository AuditLog
        {
            get { return _auditLog ??= new AuditLogRepository(_db); }
        }

        public IAuthTokenRepository AuthToken
        {
            get { return _authToken ??= new AuthTokenRepository(_db); }
        }

        public IBeginningVoucherRepository BeginningVoucher
        {
            get { return _beginningVoucher ??= new BeginningVoucherRepository(_db); }
        }

        public IBeginningVoucherDetailRepository BeginningVoucherDetail
        {
            get { return _beginningVoucherDetail ??= new BeginningVoucherDetailRepository(_db); }
        }

        public ICategoryRepository Category
        {
            get { return _category ??= new CategoryRepository(_db); }
        }

        public ICheckingVoucherRepository CheckingVoucher
        {
            get { return _checkingVoucher ??= new CheckingVoucherRepository(_db); }
        }

        public ICheckingVoucherDetailRepository CheckingVoucherDetail
        {
            get { return _checkingVoucherDetail ??= new CheckingVoucherDetailRepository(_db); }
        }

        public ICustomerRepository Customer
        {
            get { return _customer ??= new CustomerRepository(_db); }
        }

        public IPermissionRepository Permission
        {
            get { return _permission ??= new PermissionRepository(_db); }
        }

        public IProductRepository Product
        {
            get { return _product ??= new ProductRepository(_db); }
        }

        public IUserRepository User
        {
            get { return _user ??= new UserRepository(_db); }
        }

        public IUserGroupRepository UserGroup
        {
            get { return _userGroup ??= new UserGroupRepository(_db); }
        }

        public IVoucherPrefixCodeRepository VoucherPrefixCode
        {
            get { return _voucherPrefixCode ??= new VoucherPrefixCodeRepository(_db); }
        }

        public IWarehouseRepository Warehouse
        {
            get { return _warehouse ??= new WarehouseRepository(_db); }
        }

        public ITestRepository TestSetting
        {
            get { return _testSetting ??= new TestRepository(_db, _httpContextAccessor); }
        }

        public IReceiveRequestVoucherRepository ReceiveRequestVoucher
        {
            get { return _receiveRequestVoucher ??= new ReceiveRequestVoucherRepository(_db); }
        }

        public IReceiveRequestVoucherDetailRepository ReceiveRequestVoucherDetail
        {
            get { return _receiveRequestVoucherDetail ??= new ReceiveRequestVoucherDetailRepository(_db); }
        }

        public IDeliveryRequestVoucherRepository DeliveryRequestVoucher
        {
            get { return _deliveryRequestVoucher ??= new DeliveryRequestVoucherRepository(_db); }
        }

        public IDeliveryRequestVoucherDetailRepository DeliveryRequestVoucherDetail
        {
            get { return _deliveryRequestVoucherDetail ??= new DeliveryRequestVoucherDetailRepository(_db); }
        }
        
        public IFixingVoucherRepository FixingVoucher
        {
            get { return _fixingVoucher ??= new FixingVoucherRepository(_db); }
        }

        #endregion

        #region DB method

        public async Task<int> SaveChanges()
        {
            try
            {
                var now = DateTime.Now;
                var user = (AuthUser) _httpContextAccessor.HttpContext.Items["CurrentUser"];
                var userId = user?.Id ?? Guid.Empty;

                // On create
                var addedEntries = _db.ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
                foreach (var entry in addedEntries)
                {
                    // Add actor and time
                    if (entry.Entity is ISafeEntity se)
                    {
                        // se.CreatedAt = now;
                        se.CreatedById = userId;
                    }
                }

                // On update
                var modifiedEntries = _db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
                foreach (var entry in modifiedEntries)
                {
                    foreach (var pro in entry.Entity.GetType().GetProperties())
                    {
                        if (!pro.Name.Equals("IsDeleted")) continue;
                        if (entry.CurrentValues[pro.Name] is bool current &&
                            entry.OriginalValues[pro.Name] is bool original)
                        {
                            if (entry.Entity is ISafeEntity se)
                            {
                                if (!original && current)
                                {
                                    se.DeletedAt = now;
                                    se.DeletedById = userId;
                                }
                                else
                                {
                                    se.UpdatedAt = now;
                                    se.UpdatedById = userId;
                                }
                            }
                        }

                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return await _db.SaveChangesAsync();
        }

        #endregion

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}