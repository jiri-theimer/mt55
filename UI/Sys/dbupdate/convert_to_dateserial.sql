if exists (select 1 from sysobjects where  id = object_id('convert_to_dateserial') and type = 'FN')
 drop function convert_to_dateserial
GO


CREATE function [dbo].[convert_to_dateserial](@dat datetime)
RETURNS datetime
AS
BEGIN
	declare @year int,@month int,@day int

	set @year=year(@dat)
	set @month=month(@dat)
	set @day=day(@dat)

	return dbo.get_dateserial(@year,@month,@day,0,0)
END

GO