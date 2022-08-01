create or alter trigger tr_add_beginning_voucher_custom_code
    on BeginningVoucher
    instead of insert
    as
begin
    declare @new_code varchar(30), @reporting_date datetime2
    set nocount on;
    select @reporting_date = inserted.ReportingDate from inserted

    exec dbo.sp_get_voucher_code 1, @reporting_date, 'BeginningVoucher', @output = @new_code output

    insert into BeginningVoucher (Id, Code, ReportingDate, Description, WarehouseId, CreatedAt, CreatedById, UpdatedAt,
                                  UpdatedById, DeletedAt, DeletedById, IsDeleted)
    select inserted.Id,
           @new_code,
           inserted.ReportingDate,
           inserted.Description,
           inserted.WarehouseId,
           inserted.CreatedAt,
           inserted.CreatedById,
           inserted.UpdatedAt,
           inserted.UpdatedById,
           inserted.DeletedAt,
           inserted.DeletedById,
           inserted.IsDeleted
    from inserted
end


