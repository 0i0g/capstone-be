create or alter procedure sp_get_voucher_code @p_voucher smallint,
                                     @p_voucher_date datetime2,
                                     @table_name varchar(20),
                                     @output nvarchar(30) output
as
begin
    set nocount on;
    -- Params
    if @p_voucher_date is null
        set @p_voucher_date = current_timestamp

    -- Validate
    declare
        @vld_code_length smallint,
        @vld_format varchar(30)

    select @vld_code_length = Length from DocumentType where Id = @p_voucher;
    select @vld_format = Format from DocumentType where Id = @p_voucher;

    if @vld_code_length > 30
        return cast('Code length must be less than 30' as int)

    if (len(@vld_format) - len(replace(@vld_format, '[AUTO_NUM]', ''))) <> 10
        return cast('Use only 1 [AUTO_NUM]' as int)


    -- Var
    declare
        @VC varchar(10),
        @DD varchar(2),
        @MM varchar(2),
        @YY varchar(2),
        @YYYY varchar(4),
        @format varchar(30),
        @formatted varchar(30),
        @code_pattern varchar(50),
        @code_pattern_before_num varchar(30),
        @num_length smallint,
        @code_length smallint,
        @new_num smallint,
        @current_num smallint,
        @num_index smallint,
        @new_code varchar(30),
        @query nvarchar(300),
        @query_params nvarchar(100)

    -- Body
    ---- get voucher value
    set @VC = (select Prefix from VoucherPrefixCode where Id = @p_voucher)
    select @format = Format, @code_length = Length from DocumentType where Id = @p_voucher
    set @DD = IIF(day(@p_voucher_date) < 10, '0' + cast(day(@p_voucher_date) as varchar(2)),
                  cast(day(@p_voucher_date) as varchar(2)))

    ---- get date value
    set @MM = IIF(month(@p_voucher_date) < 10, '0' + cast(month(@p_voucher_date) as varchar(2)),
                  cast(month(@p_voucher_date) as varchar(2)))

    select @YY = substring(cast(year(@p_voucher_date) as varchar(4)), 3, 2)

    set @YYYY = cast(year(@p_voucher_date) as varchar(4))


    ---- generate code regex
    set @code_pattern = replace(@format, '[VC]', @VC)
    set @code_pattern = replace(@code_pattern, '[DD]', @DD)
    set @code_pattern = replace(@code_pattern, '[MM]', @MM)
    set @code_pattern = replace(@code_pattern, '[YY]', @YY)
    set @code_pattern = replace(@code_pattern, '[YYYY]', @YYYY)
    set @formatted = @code_pattern
    set @num_index = charindex('[AUTO_NUM]', @code_pattern);
    set @code_pattern_before_num = substring(@code_pattern, 1, charindex('[AUTO_NUM]', @code_pattern) - 1)

    ---- cal length
    set @num_length = @code_length - len(replace(@code_pattern, '[AUTO_NUM]', ''))

    if @num_length < 0 -- if total length less than regex length
        return cast(concat('Invalid pattern length ', @code_pattern,
                           len(replace(@code_pattern, '[AUTO_NUM]', ''))) as int)

    -- Result
    set @query_params =
            N'@num_index smallint, @num_length smallint, @code_pattern varchar(50), @output smallint output'
    set @query =
                N'select top 1 @output = cast(substring(Code, @num_index, @num_length) as int) ' +
                N'from ' + @table_name +
                ' where isnumeric(substring(Code, @num_index, @num_length)) = 1 ' +
                'and Code like replace(@code_pattern, ''[AUTO_NUM]'', ''%%'') order by Code desc';
    exec sp_executesql @query, @query_params, @num_index, @num_length, @code_pattern, @current_num output

    if @current_num is null
        set @current_num = 0

    set @new_num = @current_num + 1
    set @new_code = replace(@code_pattern, '[AUTO_NUM]', right(power(10, @num_length) + @new_num, @num_length))

    set @output = @new_code
    return;
end

