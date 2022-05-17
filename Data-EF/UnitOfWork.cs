using Data_EF.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_EF
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Repositories

        private IUserRepository _user;
        private IAuthTokenRepository _authToken;
        private IUserSettingRepository _userSetting;

        #endregion

        public UnitOfWork(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Get Repositories

        public IUserRepository User
        {
            get { return _user ??= new UserRepository(_db, _httpContextAccessor); }
        }
        
        public IAuthTokenRepository AuthToken
        {
            get { return _authToken ??= new AuthTokenRepository(_db); }
        }
        
        public IUserSettingRepository UserSetting
        {
            get
            {
                return _userSetting ??= new UserSettingRepository(_db);
            }
        }

        #endregion

        #region DB method

        public async Task<int> SaveChanges()
        {
            try
            {
                var now = DateTime.Now;
                var user = (AuthUser)_httpContextAccessor.HttpContext.Items["CurrentUser"];
                var userId = user?.Id ?? Guid.Empty;
                var currentBu = user?.CurrentBU ?? Guid.Empty;

                // On create
                var addedEntries = _db.ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
                foreach (var entry in addedEntries)
                {
                    // Add actor and time
                    if (entry.Entity is SafeEntity se)
                    {
                        // se.CreatedAt = now;
                        se.CreatedBy = userId;
                    }

                    if (entry.Entity.GetType().GetProperty("InBuId") != null)
                    {
                        entry.Entity.GetType().GetProperty("InBuId").SetValue(entry.Entity,currentBu);
                    }

                    // Add in Bu
                    // foreach (var pro in entry.Entity.GetType().GetProperties())
                    // {
                    //     if (!pro.Name.Equals("InBuId")) continue;
                    //     entry.Property("InBuId").CurrentValue = currentBu;
                    //     break;
                    // }

                    if (entry.Property("InBuId") != null)
                    {
                        entry.Property("InBuId").CurrentValue = currentBu;
                    }
                }

                // On update
                var modifiedEntries = _db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
                foreach (var entry in modifiedEntries)
                {
                    foreach (var pro in entry.Entity.GetType().GetProperties())
                    {
                        if (!pro.Name.Equals("IsDeleted")) continue;
                        if (entry.CurrentValues[pro.Name] is bool current && entry.OriginalValues[pro.Name] is bool original)
                        {
                            if (entry.Entity is SafeEntity se)
                            {
                                if (!original && current)
                                {
                                    se.DeletedAt = now;
                                    se.DeletedBy = userId;
                                }
                                else
                                {
                                    se.UpdatedAt = now;
                                    se.UpdatedBy = userId;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return await this._db.SaveChangesAsync();
        }

        #endregion

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
