using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels.Test;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TestController : BaseController
    {
        private ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }
        
        
        [Route("test")]
        [HttpPost]
        [AuthorizeRole]
        public async Task<IActionResult> GetProfile(CreateTestModel model)
        {
            return await _testService.CreateTest(model);
        }
    }
}