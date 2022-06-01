using System.Linq;
using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Data_EF.Repositories
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(AppDbContext db) : base(db)
        {
        }
    }
}