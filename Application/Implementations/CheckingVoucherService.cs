using System;
using System.Linq;
using Application.Interfaces;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Implementations
{
    public class CheckingVoucherService : BaseService, ICheckingVoucherService
    {
        private readonly ICheckingVoucherRepository _checkingVoucherRepository;
        private readonly IQueryable<CheckingVoucher> _checkingVoucherQueryable;
        private readonly IQueryable<CheckingVoucher> _checkingVoucherAllWarehouseQueryable;
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;

        public CheckingVoucherService(IServiceProvider provider) : base(provider)
        {
            _checkingVoucherRepository = _unitOfWork.CheckingVoucher;
            _checkingVoucherQueryable =
                _checkingVoucherRepository.GetMany(x => x.IsDeleted != true && x.WarehouseId == CurrentUser.Warehouse)
                    .Include(x => x.Details);
            _checkingVoucherAllWarehouseQueryable = _checkingVoucherRepository.GetMany(x => x.IsDeleted != true)
                .Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true);
        }
    }
}