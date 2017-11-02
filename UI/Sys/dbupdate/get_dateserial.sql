if exists (select 1 from sysobjects where  id = object_id('get_dateserial') and type = 'FN')
 drop function get_dateserial
GO


CREATE FUNCTION [dbo].[get_dateserial](
    @year int,
    @month int,
    @day int,
    @hour int,
    @minute int
	)
RETURNS datetime2(7)
AS
BEGIN
    RETURN
        DATEADD(MINUTE, @minute, 
        DATEADD(HOUR, @hour, 
        DATEADD(DAY, @day-1, 
        DATEADD(MONTH, @month-1, 
        DATEADD(YEAR, @year-1900, 
        CAST(CAST(0 AS datetime) AS datetime2(7)))))));
END

GO