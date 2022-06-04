using System.Threading.Tasks;
using Application.RequestModels.Test;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ITestService
    {
        Task<IActionResult> CreateTest(CreateTestModel model);
    }
}