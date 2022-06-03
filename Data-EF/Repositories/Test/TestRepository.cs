using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_EF.Repositories
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TestRepository(AppDbContext db, IHttpContextAccessor httpContextAccessor) : base(db)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Test> GetActive()
        {
            var currentUser = (AuthUser)_httpContextAccessor.HttpContext.Items["CurrentUser"];
            return _entities;
        }
    }
}