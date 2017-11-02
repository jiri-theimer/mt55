if exists (select 1 from sysobjects where  id = object_id('get_today') and type = 'FN')
 drop function get_today
GO


create FUNCTION [dbo].[get_today](
  
	)
RETURNS datetime
AS
BEGIN
    RETURN cast(convert(varchar(10), getdate(), 110) as datetime)
END

GO