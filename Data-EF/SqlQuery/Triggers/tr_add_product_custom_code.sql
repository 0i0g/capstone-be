create or alter trigger tr_add_product_custom_code
    on Product
    instead of insert
    as
begin
    declare @new_code varchar(30), @reporting_date datetime2
    set nocount on;
    select @reporting_date = inserted.CreatedAt from inserted

    exec dbo.sp_get_voucher_code 10, @reporting_date, 'Product', @output = @new_code output

    insert into Product (Id, Code, Name, IsActive, Description, OnHandMin, OnHandMax, ImageId, CreatedAt, CreatedById, UpdatedAt, UpdatedById, DeletedAt, DeletedById, IsDeleted) 
    select inserted.Id,
           @new_code,
           inserted.Name,
           inserted.IsActive,
           inserted.Description,
           inserted.OnHandMin,
           inserted.OnHandMax,
           inserted.ImageId,
           inserted.CreatedAt,
           inserted.CreatedById,
           inserted.UpdatedAt,
           inserted.UpdatedById,
           inserted.DeletedAt,
           inserted.DeletedById,
           inserted.IsDeleted
    from inserted
end
