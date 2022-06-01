using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.RequestModels.UserGroup;
using Application.ViewModels;
using Application.ViewModels.Permission;
using Application.ViewModels.UserGroup;
using Data.Entities;
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
        private readonly ILogger<UserGroupService> _logger;

        public UserGroupService(IServiceProvider provider, ILogger<UserGroupService> logger) : base(provider)
        {
            _userGroupRepository = _unitOfWork.UserGroup;
            _logger = logger;
        }

        public async Task<IActionResult> CreateUserGroup(CreateUserGroupModel model)
        {
            var userGroup = _userGroupRepository.FirstOrDefault(x => x.Name == model.Name);
            if (userGroup != null)
            {
                return ApiResponse.BadRequest(MessageConstant.UserGroupNameExisted);
            }

            _userGroupRepository.Add(new UserGroup
            {
                Name = model.Name,
                Description = model.Description
            });

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model)
        {
            var userGroup = _userGroupRepository.FirstOrDefault(x => x.Id == model.Id);
            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            // check conflict name
            if (model.Name != null)
            {
                var userGroupConflictName = _userGroupRepository.FirstOrDefault(x => x.Name == model.Name);
                if (userGroupConflictName != null && userGroupConflictName.Id != model.Id)
                {
                    return ApiResponse.BadRequest(MessageConstant.UserGroupNameExisted);
                }
            }

            userGroup.Name = model.Name ?? userGroup.Name;
            userGroup.Description = model.Description ?? userGroup.Description;

            _userGroupRepository.Update(userGroup);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateUserGroupPermission(UpdateUserGroupPermissionModel model)
        {
            var userGroup = _userGroupRepository.GetMany(x => x.Id == model.Id).Include(x => x.Permissions)
                .FirstOrDefault();
            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            var newListPermissions = model.Permissions.Select(x => new Permission
            {
                Name = x.PermissionType,
                Level = x.Level,
            }).GroupBy(x => x.Name).Select(x => x.Last()).ToList();

            userGroup.Permissions = newListPermissions;
            _userGroupRepository.Update(userGroup);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveUserGroup(Guid id)
        {
            var userGroup = _userGroupRepository.FirstOrDefault(x => x.Id == id);
            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            userGroup.IsDeleted = true;

            _userGroupRepository.Update(userGroup);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetUserGroup(Guid id)
        {
            var userGroup = _userGroupRepository.GetMany(x => x.Id == id).Select(x => new UserGroupViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.Permissions.Select(y => new PermissionViewModel()
                {
                    Name = y.Name,
                    Level = y.Level
                }).ToList()
            }).FirstOrDefault();

            if (userGroup == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserGroupNotFound);
            }

            return ApiResponse.Ok(userGroup);
        }

        public IActionResult SearchUserGroup(SearchUserGroupModel model)
        {
            var query = _userGroupRepository.GetMany(x =>
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
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchUserGroup(FetchModel model)
        {
            var userGroup = _userGroupRepository
                .GetMany(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .AsNoTracking()
                .Take(model.Size)
                .Select(x => new FetchUserGroupViewModel() {Id = x.Id, Name = x.Name})
                .ToList();

            return ApiResponse.Ok(userGroup);
        }

        public IActionResult GetAllUserGroup()
        {
            // var users = _userGroupRepository.GetActive().Select(x => new UserGroupViewModel()
            // {
            //     Id = x.Id,
            //     Name = x.Name,
            //     Description = x.Description,
            //     Permissions = x.Permissions.Select(y => new FetchPermissionViewModel()
            //     {
            //         Id = y.Id,
            //         PermissionType = y.PermissionType,
            //         Level = y.Level
            //     }).ToList()
            // }).ToList();

            return ApiResponse.Ok();
        }
    }
}