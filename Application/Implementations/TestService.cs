using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data.Enums;
using Data_EF;
using Data_EF.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.RequestModels.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Helper;

namespace Application.Implementations
{
    public class TestService : BaseService, ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TestService> _logger;

        public TestService(IServiceProvider provider, ILogger<TestService> logger) : base(provider)
        {
            _testRepository = _unitOfWork.TestSetting;
            _userRepository = _unitOfWork.User;
            _logger = logger;
        }


        public async Task<IActionResult> CreateTest(CreateTestModel model)
        {
            var user = _userRepository.FirstOrDefault(x => x.Id == model.UserId);
            if (user == null)
            {
                return ApiResponse.BadRequest(MessageConstant.UserNotFound);
            }

            var test = new Test
            {
                Name = model.Name,
                UserId = model.UserId
            };
            
            _testRepository.Add(test);
            await _unitOfWork.SaveChanges();
            
            return ApiResponse.Ok();
        }
        
        public async Task<IActionResult> Get(CreateTestModel model)
        {
            var test = new Test();
            
            return ApiResponse.Ok(test);
        }
    }
}