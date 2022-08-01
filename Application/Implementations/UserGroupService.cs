using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.Utilities;
using Application.ViewModels;
using Application.ViewModels.UserGroup;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class UserGroupService : BaseService, IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IQueryable<UserGroup> _userGroupInWarehouseQueryable;
        private readonly IQueryable<UserGroup> _userGroupInSystemQueryable;
        private readonly ILogger<UserGroupService> _logger;

        public UserGroupService(IServiceProvider provider, ILogger<UserGroupService> logger) :
            base(provider)
        {
            _userGroupRepository = _unitOfWork.UserGroup;
            _userRepository = _unitOfWork.User;
            _permissionRepository = _unitOfWork.Permission;

            _userGroupInWarehouseQueryable = _userGroupRepository.GetMany(x =>
                x.Type == EnumUserGroupType.Warehouse && x.InWarehouseId == CurrentUser.Warehouse &&
                x.IsDeleted != true);
            _userGroupInSystemQueryable =
                _userGroupRepository.GetMany(x => x.Type == EnumUserGroupType.System && x.IsDeleted != true);

            _logger = logger;
        }


        public async Task<IActionResult> CreateUserGroup(CreateUserGroupModel model, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroup = queryable.FirstOrDefault(x => x.Name == model.Name);
            if (userGroup != null)
            {
                return ApiResponse.BadRequest(MessageConstant.UserGroupNameExisted);
            }

            _userGroupRepository.Add(new UserGroup
            {
                Name = model.Name,
                Description = model.Description,
                Type = type,
                InWarehouseId = type == EnumUserGroupType.Warehouse ? CurrentUser.Warehouse!.Value : null,
                Permissions = PermissionManager.GetValidWarehousePermission(model.Permissions)
                    .Select(x => new Permission {Type = x}).ToList()
            });

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroup = queryable.Include(x=>x.Permissions).FirstOrDefault(x => x.Id == model.Id);
            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            if (userGroup.CanUpdate != true)
            {
                return ApiResponse.BadRequest(MessageConstant.CannotUpdateDefaultUserGroup);
            }

            // check conflict name
            if (model.Name != null && model.Name != userGroup.Name)
            {
                var userGroupConflictName = queryable.FirstOrDefault(x => x.Name == model.Name);
                if (userGroupConflictName != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.UserGroupNameExisted);
                }
            }

            userGroup.Name = model.Name ?? userGroup.Name;
            userGroup.Description = model.Description ?? userGroup.Description;
            userGroup.Permissions = PermissionManager.GetValidWarehousePermission(model.Permissions)
                .Select(x => new Permission {Type = x}).ToList();

            _userGroupRepository.Update(userGroup);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> AddUserInToGroup(AddUserInToGroupModel model, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroup = queryable.FirstOrDefault(x => x.Id == model.GroupId);
            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            User user;
            if (type == EnumUserGroupType.Warehouse)
            {
                user = _userRepository
                    .GetMany(x => x.Id == model.UserId && x.InWarehouseId == CurrentUser.Warehouse!.Value)
                    .Include(x => x.UserInGroups).FirstOrDefault();
            }
            else
            {
                user = _userRepository.GetMany(x => x.Id == model.UserId).Include(x => x.UserInGroups).FirstOrDefault();
            }

            if (user == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserNotFound);
            }

            if (user.UserInGroups.Any(x => x.GroupId == userGroup.Id))
            {
                return ApiResponse.Conflict(MessageConstant.DuplicateUserGroup);
            }

            userGroup.UserInGroups.Add(new UserInGroup {UserId = user.Id});
            _userGroupRepository.Update(userGroup);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulUserGroup(List<Guid> ids, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var defaultGroups = queryable.Where(x => ids.Contains(x.Id) && x.CanUpdate == false).ToList();
            if (defaultGroups.Count > 0)
            {
                return ApiResponse.BadRequest(
                    MessageConstant.CannotDeleteDefaultUserGroup.WithValues(
                        string.Join(", ", defaultGroups.Select(x => x.Name))));
            }

            var userGroups = queryable.Where(x => ids.Contains(x.Id)).Include(x => x.UserInGroups).ToList();

            var groupsHaveUser = userGroups.Where(x => x.UserInGroups.Count > 0).ToList();
            if (groupsHaveUser.Count > 0)
            {
                return ApiResponse.BadRequest(
                    MessageConstant.CannotRemoveMulUserGroupContainUser.WithValues(
                        string.Join(", ", groupsHaveUser.Select(x => x.Name))));
            }

            userGroups.ForEach(x => { x.IsDeleted = true; });

            _userGroupRepository.UpdateRange(userGroups);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetUserGroup(Guid id, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroup = queryable.Where(x => x.Id == id).Select(x => new UserGroupViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.Permissions.Select(y => y.Type).ToList()
            }).FirstOrDefault();

            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            return ApiResponse.Ok(userGroup);
        }

        public IActionResult SearchUserGroup(SearchUserGroupModel model, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var query = queryable.Where(x =>
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

            var data = query.Select(x => new SearchUserGroupViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.Permissions.Select(x => x.Type).ToList(),
                CanUpdate = x.CanUpdate.Value
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchUserGroup(FetchModel model, EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroups = queryable
                .Where(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .AsNoTracking()
                .Take(model.Size)
                .Select(x => new FetchUserGroupViewModel() {Id = x.Id, Name = x.Name})
                .ToList();

            return ApiResponse.Ok(userGroups);
        }

        public IActionResult GetAllUserGroup(EnumUserGroupType type)
        {
            var queryable = type == EnumUserGroupType.Warehouse
                ? _userGroupInWarehouseQueryable
                : _userGroupInSystemQueryable;

            var userGroups = queryable.ToList();

            return ApiResponse.Ok(userGroups);
        }

        public IActionResult GetAllPermission(EnumUserGroupType type)
        {
            var pers = PermissionManager.WarehousePermissions;

            return ApiResponse.Ok(pers);
        }
    }
}