using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IActionResult> CreateCustomer(CreateCustomerModel model);

        IActionResult SearchCustomers(SearchCustomersModel model);

        IActionResult FetchCustomers(FetchModel model);

        Task<IActionResult> UpdateCustomer(UpdateCustomerModel model);

        Task<IActionResult> RemoveCustomer(RemoveModel model);

        IActionResult GetCustomer(Guid id);

        IActionResult GetAllCustomers();
    }
}