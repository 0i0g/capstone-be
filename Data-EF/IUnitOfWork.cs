using Data_EF.Repositories;
using System.Threading.Tasks;

namespace Data_EF
{
    public interface IUnitOfWork
    {
        #region Repositories

        public IUserRepository User { get; }

        public IAuthTokenRepository AuthToken { get; }

        public IUserSettingRepository UserSetting { get; }

        #endregion

        #region DB method

        Task<int> SaveChanges();

        #endregion
    }
}