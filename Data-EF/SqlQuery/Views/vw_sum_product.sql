create or alter view vw_sum_product as
select sv.ProductId, p.Name as ProductName, sum(sv.Quantity) Quantity, sv.WarehouseId
from (
         -- Beginning
         select bvd.ProductId, sum(bvd.Quantity) as Quantity, bv.WarehouseId
         from BeginningVoucher bv
                  left join BeginningVoucherDetail bvd on bv.Id = bvd.VoucherId and bv.IsDeleted = 0
         group by bvd.ProductId, WarehouseId

         union all
         -- Receive
         select rvd.ProductId, sum(rvd.Quantity) as Quantity, rv.WarehouseId
         from ReceiveVoucher rv
                  left join ReceiveVoucherDetail rvd on rv.Id = rvd.VoucherId and rv.IsDeleted = 0
         group by rvd.ProductId, WarehouseId

         union all
         -- Delivery
         select dvd.ProductId, sum(dvd.Quantity) * -1 as Quantity, dv.WarehouseId
         from DeliveryVoucher dv
                  left join DeliveryVoucherDetail dvd on dv.Id = dvd.VoucherId and dv.IsDeleted = 0
         group by dvd.ProductId, dv.WarehouseId

         union all
         -- Transfer in
         select tvd.ProductId, sum(tvd.Quantity) as Quantity, tv.InboundWarehouseId as WarehouseId
         from TransferVoucher tv
                  left join TransferVoucherDetail tvd on tv.Id = tvd.VoucherId and tv.IsDeleted = 0
         group by tvd.ProductId, tv.InboundWarehouseId

         union all
         -- Transfer out
         select tvd.ProductId, sum(tvd.Quantity) * -1 as Quantity, tv.OutboundWarehouseId as WarehouseId
         from TransferVoucher tv
                  left join TransferVoucherDetail tvd on tv.Id = tvd.VoucherId and tv.IsDeleted = 0
         group by tvd.ProductId, tv.OutboundWarehouseId

         union all
         -- Fixing
         select fvd.ProductId, sum(fvd.Quantity) as Quantity, fv.WarehouseId
         from FixingVoucher fv
                  left join FixingVoucherDetail fvd on fv.Id = fvd.VoucherId and fv.IsDeleted = 0
         group by fvd.ProductId, fv.WarehouseId) sv
         left join Product p on sv.ProductId = p.Id
group by sv.ProductId, sv.WarehouseId, p.Name
