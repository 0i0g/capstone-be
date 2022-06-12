using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;

namespace Application.Implementations
{
    public class VoucherPrefixCodeService : BaseService, IVoucherPrefixCodeService
    {
        private readonly IVoucherPrefixCodeRepository _voucherPrefixCodeRepository;

        public VoucherPrefixCodeService(IServiceProvider provider) : base(provider)
        {
            _voucherPrefixCodeRepository = _unitOfWork.VoucherPrefixCode;
        }

        public async Task<IActionResult> UpdateVoucherPrefixCode(UpdateVoucherPrefixCodeModel model)
        {
            var voucherPrefixCode = _voucherPrefixCodeRepository.FirstOrDefault(x => x.Id == model.Id);
            if (voucherPrefixCode == null)
            {
                return ApiResponse.NotFound(MessageConstant.VoucherPrefixCodeNotFound);
            }

            voucherPrefixCode.Prefix = model.Prefix ?? voucherPrefixCode.Prefix;
            _voucherPrefixCodeRepository.Update(voucherPrefixCode);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetAllVoucherPrefix()
        {
            var prefixs = _voucherPrefixCodeRepository.GetAll();
            return ApiResponse.Ok(prefixs);
        }
    }
}