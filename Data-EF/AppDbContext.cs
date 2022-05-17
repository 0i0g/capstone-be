using System;
using Data.Entities;
using Data.Enums;
using Data_EF.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Utilities.Helper;

namespace Data_EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Add filter safe delete

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                if (typeof(SafeEntity).IsAssignableFrom(entityType.ClrType))
                    entityType.AddSoftDeleteQueryFilter();

            #endregion

            #region Reference

            #endregion

            #region Rename table


            #endregion

            #region Default value

            // window 
            // @formatter:off
            modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"../Data-EF/SeedData/User.json"));
            // @formatter:on

            // linux
            modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"SeedData/User.json"));

            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}