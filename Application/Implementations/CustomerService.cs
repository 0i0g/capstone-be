using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.Customer;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class CustomerService: BaseService, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IQueryable<Customer> _customerQueryable;
        
        public CustomerService(IServiceProvider provider) : base(provider)
        {
            _customerRepository = _unitOfWork.Customer;
            _customerQueryable = _customerRepository.GetMany(x => x.IsDeleted != true);
        }

        public async Task<IActionResult> CreateCustomer(CreateCustomerModel model)
        {
            var customer = _customerQueryable.FirstOrDefault(x => x.Name == model.Name);
            if (customer != null)
            {
                return ApiResponse.BadRequest(MessageConstant.CustomerNameExisted);
            }
            
            var newCustomer = new Customer
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Description = model.Description
            };

            _customerRepository.Add(newCustomer);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult SearchCustomers(SearchCustomersModel model)
        {
            var query = _customerQueryable.Where(x =>
                string.IsNullOrWhiteSpace(model.Name) || x.Name.Contains(model.Name)).AsNoTracking();

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "NAME":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Name).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Name).ThenByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(MessageConstant.OrderByInvalid.WithValues("Name, CreatedAt"));
            }

            var data = query.Select(x => new CustomerViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Description = x.Description,
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchCustomers(FetchModel model)
        {
            var customers = _customerQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchCustomerViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            return ApiResponse.Ok(customers);
        }

        public async Task<IActionResult> UpdateCustomer(UpdateCustomerModel model)
        {
            var customer = _customerQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (customer == null) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);
            
            if (model.Name != null && model.Name != customer.Name)
            {
                var customerConflictName = _customerQueryable.FirstOrDefault(x => x.Name == model.Name);
                if (customerConflictName != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.CustomerNameExisted);
                }
            }

            customer.Name = model.Name ?? customer.Name;
            customer.Address = model.Address ?? customer.Address;
            customer.Phone = model.Phone ?? customer.Phone;
            customer.Email = model.Email ?? customer.Email;
            customer.Description = model.Description ?? customer.Description;

            _customerRepository.Update(customer);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveCustomer(RemoveModel model)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetCustomer(Guid id)
        {
            var customer = _customerQueryable.Select(x => new CustomerViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Description = x.Description
            }).FirstOrDefault(x => x.Id == id);
            if (customer == null) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);

            return ApiResponse.Ok(customer);
        }

        public IActionResult GetAllCustomers()
        {
            var customers = _customerQueryable.Select(x => new CustomerViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Description = x.Description
            }).ToList();

            return ApiResponse.Ok(customers);
        }
    }
}