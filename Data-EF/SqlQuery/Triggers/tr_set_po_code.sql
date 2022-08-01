-- drop trigger tr_add_po_custom_code
-- go

create trigger tr_add_po_custom_code
    on Po
    instead of INSERT
    as
begin
    declare @is_request bit,
        @voucher_id smallint,
        @new_code varchar(30),
        @voucher_date datetime2

    set nocount on;

    select @is_request = inserted.IsPORequest, @voucher_date = inserted.OrderDate from inserted

    if @is_request = 1
        set @voucher_id = 2
    else
        set @voucher_id = 3

    exec dbo.sp_get_voucher_code @voucher_id, @voucher_date, 'Po', @output = @new_code output

    insert into Po (Id, Code, Description, OrderDate, PORequestStatus, POStatus, IsPORequest, PORequestId, CustomerId,
                    InBuId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, DeletedAt, DeletedBy, IsDeleted)
    select inserted.Id,
           @new_code,
           inserted.Description,
           inserted.OrderDate,
           inserted.PORequestStatus,
           inserted.POStatus,
           inserted.IsPORequest,
           inserted.PORequestId,
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
