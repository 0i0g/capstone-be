using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.Warehouse;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IQueryable<Warehouse> _warehouseQueryable;

        public WarehouseService(IServiceProvider provider) : base(provider)
        {
            _warehouseRepository = _unitOfWork.Warehouse;
            _warehouseQueryable = _warehouseRepository.GetMany(x => x.IsDeleted != true);
        }

        public async Task<IActionResult> CreateWarehouse(CreateWarehouseModel model)
        {
            var warehouse = _warehouseQueryable.FirstOrDefault(x => x.Name == model.Name);
            if (warehouse != null)
            {
                return ApiResponse.BadRequest(MessageConstant.WarehouseNameExisted);
            }
            
            var newWarehouse = new Warehouse
            {
                Name = model.Name,
                Address = model.Address,
            };

            _warehouseRepository.Add(newWarehouse);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }
        
        public IActionResult SearchWarehouses(SearchWarehousesModel model)
        {
            var query = _warehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Name) || x.Name.Contains(model.Name)) &&
                (string.IsNullOrWhiteSpace(model.Address) || x.Address.Contains(model.Address)));

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
                case "ADDRESS":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Address).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Address).ThenByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Name, CreatedAt, Address"));
            }
            
            var data = query.Select(x => new WarehouseViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchWarehouses(FetchModel model)
        {
            var warehouses = _warehouseQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchWarehouseViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            return ApiResponse.Ok(warehouses);
        }

        public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseModel model)
        {
            var warehouse = _warehouseQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (warehouse == null) return ApiResponse.BadRequest(MessageConstant.WarehouseNotFound);
            
            if (model.Name != null && model.Name != warehouse.Name)
            {
                var warehouseConflictName = _warehouseQueryable.FirstOrDefault(x => x.Name == model.Name);
                if (warehouseConflictName != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.WarehouseNameExisted);
                }
            }

            warehouse.Name = model.Name ?? warehouse.Name;
            warehouse.Address = model.Address ?? warehouse.Address;

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveWarehouse(RemoveModel model)
        {
            var warehouse = _warehouseQueryable.Where(x => x.Id == model.Id)
                .Include(x=>x.UserGroups)
                .Include(x=>x.Users)
                .Include(x=>x.BeginningVouchers)
                .Include(x=>x.CheckingVouchers)
                .FirstOrDefault();
            if (warehouse == null) return ApiResponse.BadRequest(MessageConstant.WarehouseNotFound);

            if (warehouse.UserGroups.Count > 0 && model.ForceRemoveRef == false)
            {
                return ApiResponse.NotFound(MessageConstant.CannotRemoveUserGroupContainUser);
            }

            if (warehouse.UserGroups.Count > 0 && model.ForceRemoveRef)
            {
                foreach (var x in warehouse.UserGroups)
                {
                    x.IsDeleted = true;
                }
                foreach (var x in warehouse.Users)
                {
                    x.IsDeleted = true;
                }
                foreach (var x in warehouse.BeginningVouchers)
                {
                    x.IsDeleted = true;
                }

                foreach (var x in warehouse.CheckingVouchers)
                {
                    x.IsDeleted = true;
                }
            }

            warehouse.IsDeleted = true;

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetWarehouse(Guid id)
        {
            var warehouse = _warehouseQueryable.Select(x => new WarehouseViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address
            }).FirstOrDefault(x => x.Id == id);
            if (warehouse == null) return ApiResponse.BadRequest(MessageConstant.WarehouseNotFound);

            return ApiResponse.Ok(warehouse);
        }
        
        public IActionResult GetAllWarehouses()
        {
            var warehouses = _warehouseQueryable.Select(x => new WarehouseViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address
            }).ToList();

            return ApiResponse.Ok(warehouses);
        }
    }
}