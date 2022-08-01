create trigger tr_add_voucher_custom_code
    on Voucher
    instead of INSERT
    as
begin
    set nocount on;
    declare @voucher_type varchar(30),
        @voucher_id smallint,
        @new_code varchar(30),
        @voucher_date datetime2,
        @table_name varchar(20)

    select @voucher_type = inserted.type, @voucher_date = inserted.VoucherDate from inserted

    set @voucher_id =
            case
                when @voucher_type = 'WAREHOUSE_TRANSFER_REQUEST'
                    then 4
                when @voucher_type = 'WAREHOUSE_TRANSFER'
                    then 5
                when @voucher_type = 'GOODS_RECEIPT'
                    then 6
                when @voucher_type = 'GOODS_RECEIPT_PO'
                    then 7
                when @voucher_type = 'GOODS_RECEIPT_SO'
                    then 8
                when @voucher_type = 'GOODS_ISSUE_PO_REQUEST'
                    then 9
                when @voucher_type = 'GOODS_ISSUE_PO'
                    then 10
                when @voucher_type = 'DELIVERY_ORDER_REQUEST'
                    then 11
                when @voucher_type = 'DELIVERY_ORDER'
                    then 12
                when @voucher_type = 'INTERNAL_MATERIAL_TRANSFER'
                    then 13
                end;
    set @table_name = 'Voucher'

    exec dbo.sp_get_voucher_code @voucher_id, @voucher_date, @table_name, @output = @new_code output

    insert into Voucher (Id, Code, VoucherDate, DeliveryDate, Description, Locked, Type, Status, SoId, PoId, CustomerId,
                         InboundWarehouseId, OutboundWarehouseId, ParentId, EmployeeId, DeliveryId, CreatedAt,
                         CreatedBy, UpdatedAt, UpdatedBy, DeletedAt, DeletedBy, IsDeleted)
    select i.Id,
           @new_code,
           i.VoucherDate,
           i.DeliveryDate,
           i.Description,
           i.Locked,
           i.Type,
           i.Status,
           i.SoId,
           i.PoId,
           i.CustomerId,
           i.InboundWarehouseId,
           i.OutboundWarehouseId,
           i.ParentId,
           i.EmployeeId,
           i.DeliveryId,
           i.CreatedAt,
           i.CreatedBy,
           i.UpdatedAt,
           i.UpdatedBy,
           i.DeletedAt,
           i.DeletedBy,
           i.IsDeleted
    from inserted as i

end
