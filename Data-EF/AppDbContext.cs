using System;
using System.Xml;
using Data.Entities;
using Data_EF.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Utilities.Helper;

namespace Data_EF
{
    public class AppDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            _loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
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

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.Type).HasConversion<string>();
            });

            modelBuilder.Entity<AuthToken>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.User).WithMany(x => x.AuthTokens).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BeginningVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.BeginningVouchers).HasForeignKey(x => x.WarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BeginningVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details).HasForeignKey(x => x.VoucherId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Product).WithMany(x => x.BeginningVoucherDetails).HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<CheckingVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.CheckingVouchers).HasForeignKey(x => x.WarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CheckingVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");


                entity.HasOne(x => x.Voucher).WithMany(x => x.Details).HasForeignKey(x => x.VoucherId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Product).WithMany(x => x.CheckingVoucherDetails).HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Code).IsUnique();
                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<DeliveryRequestVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd().Metadata
                    .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Customer).WithMany(x => x.DeliveryRequestVouchers)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Warehouse).WithMany(x => x.DeliveryRequestVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<DeliveryRequestVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Product).WithMany(x => x.DeliveryRequestVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FixingVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.FixingVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FixingVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.Property(x => x.Type).HasConversion<string>();

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Product).WithMany(x => x.FixingVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(x => new {x.Type, x.UserGroupId});
                entity.HasOne(x => x.UserGroup).WithMany(x => x.Permissions).HasForeignKey(x => x.UserGroupId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.Inc).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Code).IsUnique();
                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(x => new {x.ProductId, x.CategoryId});
                entity.HasOne(x => x.Product).WithMany(x => x.ProductCategories).HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Category).WithMany(x => x.ProductCategories).HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ReceiveRequestVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Inc).ValueGeneratedOnAdd().Metadata
                    .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Customer).WithMany(x => x.ReceiveRequestVouchers)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Warehouse).WithMany(x => x.ReceiveRequestVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ReceiveRequestVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Product).WithMany(x => x.ReceiveRequestVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            // modelBuilder.Entity<TransferRequestVoucher>(entity =>
            // {
            //     // TODO implement
            // });
            //
            // modelBuilder.Entity<TransferRequestVoucherDetail>(entity =>
            // {
            //     // TODO implement
            // });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.Gender).HasConversion<string>();
                entity.HasIndex(x => x.Username).IsUnique();
                entity.HasIndex(x => x.Email).IsUnique();

                entity.HasOne(x => x.InWarehouse).WithMany(x => x.Users).HasForeignKey(x => x.InWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Avatar).WithOne(x => x.User).HasForeignKey<User>(x => x.AvatarId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.CanUpdate).HasDefaultValue(true);
                entity.Property(x => x.Type).HasConversion<string>();
                entity.HasIndex(x => new {x.Name, x.InWarehouseId}).IsUnique();

                entity.HasOne(x => x.InWarehouse).WithMany(x => x.UserGroups).HasForeignKey(x => x.InWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserInGroup>(entity =>
            {
                entity.HasKey(x => new {x.UserId, x.GroupId});
                entity.HasOne(x => x.User).WithMany(x => x.UserInGroups).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Group).WithMany(x => x.UserInGroups).HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<VoucherPrefixCode>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(x => x.VoucherName).IsUnique();
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(x => x.Name).IsUnique();
            });

            #endregion

            #region Rename table

            #endregion

            #region Default value

            // window 
            // @formatter:off
            modelBuilder.Entity<Permission>().HasData(DataHelper.ReadSeedData<Permission>(@"../Data-EF/SeedData/Permission.json"));
            modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"../Data-EF/SeedData/User.json"));
            modelBuilder.Entity<UserGroup>().HasData(DataHelper.ReadSeedData<UserGroup>(@"../Data-EF/SeedData/UserGroup.json"));
            modelBuilder.Entity<UserInGroup>().HasData(DataHelper.ReadSeedData<UserInGroup>(@"../Data-EF/SeedData/UserInGroup.json"));
            modelBuilder.Entity<Warehouse>().HasData(DataHelper.ReadSeedData<Warehouse>(@"../Data-EF/SeedData/Warehouse.json"));
            // @formatter:on

            // linux
            // modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(@"SeedData/User.json"));

            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}