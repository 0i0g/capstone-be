-- drop trigger tr_add_so_custom_code
-- go

create trigger tr_add_so_custom_code
    on So
    instead of INSERT
    as
begin
    declare @new_code varchar(30), @voucher_date datetime2
    set nocount on;

    select @voucher_date = inserted.OrderDate from inserted

    exec dbo.sp_get_voucher_code 1, @voucher_date, 'So', @output = @new_code output

    insert into So (Id, Code, POCode, Description, OrderDate, DeliveryDate, Status, CustomerId, InBuId, CreatedAt,
                    CreatedBy, UpdatedAt, UpdatedBy, DeletedAt, DeletedBy, IsDeleted)
    select inserted.Id,
           @new_code,
           inserted.POCode,
           inserted.Description,
           inserted.OrderDate,
           inserted.DeliveryDate,
           inserted.Status,
           inserted.CustomerId,
           inserted.InBuId,
           inserted.CreatedAt,
           inserted.CreatedBy,
           inserted.UpdatedAt,
           inserted.UpdatedBy,
           inserted.DeletedAt,
           inserted.DeletedBy,
           inserted.IsDeleted
    from inserted
end