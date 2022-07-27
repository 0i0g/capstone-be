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

                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.BeginningVouchers).HasForeignKey(x => x.WarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.CreatedBy).WithMany(x => x.BeginningVouchers)
                    .HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.DeletedBy).WithMany(x => x.BeginningVouchersDeleted)
                    .HasForeignKey(x => x.DeletedById).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BeginningVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details).HasForeignKey(x => x.VoucherId)
                    .OnDelete(DeleteBehavior.Cascade);
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

                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.CheckingVouchers).HasForeignKey(x => x.WarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.CreatedBy).WithMany(x => x.CheckingVouchers)
                    .HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.DeletedBy).WithMany(x => x.CheckingVouchersDeleted)
                    .HasForeignKey(x => x.DeletedById).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CheckingVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");


                entity.HasOne(x => x.Voucher).WithMany(x => x.Details).HasForeignKey(x => x.VoucherId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.CheckingVoucherDetails).HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(x => x.Code).IsUnique();
                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<DeliveryRequestVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

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
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.DeliveryRequestVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<DeliveryVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);
                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Customer).WithMany(x => x.DeliveryVouchers)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Warehouse).WithMany(x => x.DeliveryVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<DeliveryVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.DeliveryVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FixingVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(x => x.Warehouse).WithMany(x => x.FixingVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FixingVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.Property(x => x.Type).HasConversion<string>();

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.FixingVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(x => new { x.Type, x.UserGroupId });
                entity.HasOne(x => x.UserGroup).WithMany(x => x.Permissions).HasForeignKey(x => x.UserGroupId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(x => new { x.ProductId, x.CategoryId });
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

                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);

                entity.HasOne(x => x.Customer).WithMany(x => x.ReceiveRequestVouchers)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Warehouse).WithMany(x => x.ReceiveRequestVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ReceiveRequestVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.ReceiveRequestVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ReceiveVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);

                entity.HasOne(x => x.Customer).WithMany(x => x.ReceiveVouchers)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Warehouse).WithMany(x => x.ReceiveVouchers)
                    .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ReceiveVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.ReceiveVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TransferRequestVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);

                entity.HasOne(x => x.InboundWarehouse).WithMany(x => x.InboundTransferRequestVouchers)
                    .HasForeignKey(x => x.InboundWarehouseId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.OutboundWarehouse).WithMany(x => x.OutboundTransferRequestVouchers)
                    .HasForeignKey(x => x.OutboundWarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TransferRequestVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.TransferRequestVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TransferVoucher>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.Status).HasConversion<string>();
                entity.Property(x => x.Locked).HasDefaultValue(false);

                entity.HasOne(x => x.InboundWarehouse).WithMany(x => x.InboundTransferVouchers)
                    .HasForeignKey(x => x.InboundWarehouseId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.OutboundWarehouse).WithMany(x => x.OutboundTransferVouchers)
                    .HasForeignKey(x => x.OutboundWarehouseId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TransferVoucherDetail>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(x => x.Voucher).WithMany(x => x.Details)
                    .HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product).WithMany(x => x.TransferVoucherDetails)
                    .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.Gender).HasConversion<string>();

                entity.HasOne(x => x.InWarehouse).WithMany(x => x.Users).HasForeignKey(x => x.InWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
                // entity.HasOne(x => x.Avatar).WithOne(x => x.User).HasForeignKey<User>(x => x.AvatarId)
                //     .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.CreatedBy).WithMany(x => x.UsersCreated)
                    .HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.DeletedBy).WithMany(x => x.UsersDeleted)
                    .HasForeignKey(x => x.DeletedById).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(x => x.IsDeleted).HasDefaultValue(false);

                entity.Property(x => x.CanUpdate).HasDefaultValue(true);
                entity.Property(x => x.Type).HasConversion<string>();
                entity.HasIndex(x => new { x.Name, x.InWarehouseId }).IsUnique();

                entity.HasOne(x => x.InWarehouse).WithMany(x => x.UserGroups).HasForeignKey(x => x.InWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserInGroup>(entity =>
            {
                entity.HasKey(x => new { x.UserId, x.GroupId });
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

                entity.HasOne(x => x.CreatedBy).WithMany(x => x.WarehousesCreated)
                    .HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.DeletedBy).WithMany(x => x.WarehousesDeleted)
                    .HasForeignKey(x => x.DeletedById).OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Rename table

            #endregion

            #region Default value

            // @formatter:off
            modelBuilder.Entity<Permission>().HasData(DataHelper.ReadSeedData<Permission>(DataHelper.MapPath("SeedData/Permission.json")));
            modelBuilder.Entity<Customer>().HasData(DataHelper.ReadSeedData<Customer>(DataHelper.MapPath("SeedData/Customer.json")));
            modelBuilder.Entity<User>().HasData(DataHelper.ReadSeedData<User>(DataHelper.MapPath("SeedData/User.json")));
            modelBuilder.Entity<UserGroup>().HasData(DataHelper.ReadSeedData<UserGroup>(DataHelper.MapPath("SeedData/UserGroup.json")));
            modelBuilder.Entity<UserInGroup>().HasData(DataHelper.ReadSeedData<UserInGroup>(DataHelper.MapPath("SeedData/UserInGroup.json")));
            modelBuilder.Entity<Warehouse>().HasData(DataHelper.ReadSeedData<Warehouse>(DataHelper.MapPath("SeedData/Warehouse.json")));
            modelBuilder.Entity<Product>().HasData(DataHelper.ReadSeedData<Product>(DataHelper.MapPath("SeedData/Product.json")));
            modelBuilder.Entity<BeginningVoucher>().HasData(DataHelper.ReadSeedData<BeginningVoucher>(DataHelper.MapPath("SeedData/BeginningVoucher.json")));
            modelBuilder.Entity<BeginningVoucherDetail>().HasData(DataHelper.ReadSeedData<BeginningVoucherDetail>(DataHelper.MapPath("SeedData/BeginningVoucherDetail.json")));
            modelBuilder.Entity<CheckingVoucher>().HasData(DataHelper.ReadSeedData<CheckingVoucher>(DataHelper.MapPath("SeedData/CheckingVoucher.json")));
            modelBuilder.Entity<CheckingVoucherDetail>().HasData(DataHelper.ReadSeedData<CheckingVoucherDetail>(DataHelper.MapPath("SeedData/CheckingVoucherDetail.json")));
            modelBuilder.Entity<DeliveryRequestVoucher>().HasData(DataHelper.ReadSeedData<DeliveryRequestVoucher>(DataHelper.MapPath("SeedData/DeliveryRequestVoucher.json")));
            modelBuilder.Entity<DeliveryRequestVoucherDetail>().HasData(DataHelper.ReadSeedData<DeliveryRequestVoucherDetail>(DataHelper.MapPath("SeedData/DeliveryRequestVoucherDetail.json")));
            modelBuilder.Entity<FixingVoucher>().HasData(DataHelper.ReadSeedData<FixingVoucher>(DataHelper.MapPath("SeedData/FixingVoucher.json")));
            modelBuilder.Entity<FixingVoucherDetail>().HasData(DataHelper.ReadSeedData<FixingVoucherDetail>(DataHelper.MapPath("SeedData/FixingVoucherDetail.json")));
            modelBuilder.Entity<ReceiveRequestVoucher>().HasData(DataHelper.ReadSeedData<ReceiveRequestVoucher>(DataHelper.MapPath("SeedData/ReceiveRequestVoucher.json")));
            modelBuilder.Entity<ReceiveRequestVoucherDetail>().HasData(DataHelper.ReadSeedData<ReceiveRequestVoucherDetail>(DataHelper.MapPath("SeedData/ReceiveRequestVoucherDetail.json")));
            modelBuilder.Entity<TransferRequestVoucher>().HasData(DataHelper.ReadSeedData<TransferRequestVoucher>(DataHelper.MapPath("SeedData/TransferRequestVoucher.json")));
            modelBuilder.Entity<TransferRequestVoucherDetail>().HasData(DataHelper.ReadSeedData<TransferRequestVoucherDetail>(DataHelper.MapPath("SeedData/TransferRequestVoucherDetail.json")));
            modelBuilder.Entity<DeliveryVoucher>().HasData(DataHelper.ReadSeedData<DeliveryVoucher>(DataHelper.MapPath("SeedData/DeliveryVoucher.json")));
            modelBuilder.Entity<DeliveryVoucherDetail>().HasData(DataHelper.ReadSeedData<DeliveryVoucherDetail>(DataHelper.MapPath("SeedData/DeliveryVoucherDetail.json")));
            modelBuilder.Entity<ReceiveVoucher>().HasData(DataHelper.ReadSeedData<ReceiveVoucher>(DataHelper.MapPath("SeedData/ReceiveVoucher.json")));
            modelBuilder.Entity<ReceiveVoucherDetail>().HasData(DataHelper.ReadSeedData<ReceiveVoucherDetail>(DataHelper.MapPath("SeedData/ReceiveVoucherDetail.json")));
            modelBuilder.Entity<TransferVoucher>().HasData(DataHelper.ReadSeedData<TransferVoucher>(DataHelper.MapPath("SeedData/TransferVoucher.json")));
            modelBuilder.Entity<TransferVoucherDetail>().HasData(DataHelper.ReadSeedData<TransferVoucherDetail>(DataHelper.MapPath("SeedData/TransferVoucherDetail.json")));

            // @formatter:on

            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}