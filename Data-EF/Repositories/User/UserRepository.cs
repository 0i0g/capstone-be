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
        public UserRepository(AppDbContext db) : base(db)
        {
        }
    }
}