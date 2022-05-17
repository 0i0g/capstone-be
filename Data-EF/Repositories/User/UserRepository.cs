using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_EF.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(AppDbContext db, IHttpContextAccessor httpContextAccessor) : base(db)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<User> GetActive()
        {
            var currentUser = (AuthUser)_httpContextAccessor.HttpContext.Items["CurrentUser"];
            return _entities;
        }
    }
}