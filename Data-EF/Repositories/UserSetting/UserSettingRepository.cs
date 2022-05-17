using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_EF.Repositories
{
    public class UserSettingRepository :  Repository<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(AppDbContext db) : base(db)
        {
        }
    }
}
