using System;
using Data.Entities;
using Data.Enums;
using Data_EF.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
                if (typeof(ISafeEntity).IsAssignableFrom(entityType.ClrType))
                    entityType.AddSoftDeleteQueryFilter();

            #endregion

            #region Reference

            modelBuilder.Entity<Attachment>(entity => { entity.Property(x => x.Id).HasDefaultValueSql("NEWID()"); });

            modelBuilder.Entity<AuthToken>(entity => { entity.Property(x => x.Id).HasDefaultValueSql("NEWID()"); });

            modelBuilder.Entity<Permission>(entity => { entity.Property(x => x.Id).HasDefaultValueSql("NEWID()"); });
            
            modelBuilder.Entity<Test>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(x => x.User).WithOne(x => x.Test).HasForeignKey<Test>(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.UserSetting).WithOne(x => x.User).HasForeignKey<UserSetting>(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(x => x.UserInGroups).WithOne(x => x.User).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction); 
                entity.HasMany(x => x.AuthTokens).WithOne(x => x.User).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Avatar).WithOne(x => x.User).HasForeignKey<User>(x => x.AvatarId);                

                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(x => x.Username).IsUnique();
                entity.HasIndex(x => x.Email).IsUnique();
                entity.Property(x => x.Gender).HasConversion<string>();
                entity.Property(x => x.Confirmed).HasDefaultValue(true);
                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasMany(x => x.Permissions).WithOne(x => x.Group).HasForeignKey(x => x.GroupId);

                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<UserInGroup>(entity =>
            {
                entity.HasKey(x => new {x.UserId, x.GroupId});
                entity.HasOne(x => x.User).WithMany(x => x.UserInGroups).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Group).WithMany(x => x.UserInGroups).HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserSetting>(entity => { entity.Property(x => x.Id).HasDefaultValueSql("NEWID()"); });

            #endregion

            #region Rename table

            #endregion

            #region Default value

            // window 
            // @formatter:off
            modelBuilder.Entity<Attachment>().HasData(DataHelper.ReadSeedData<Attachment>(@"../Data-EF/SeedData/Attachment.json"));
            modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"../Data-EF/SeedData/User.json"));
            // @formatter:on

            // linux
            // modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"SeedData/User.json"));

            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}