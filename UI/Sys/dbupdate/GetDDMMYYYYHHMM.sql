if exists (select 1 from sysobjects where  id = object_id('GetDDMMYYYYHHMM') and type = 'FN')
 drop function GetDDMMYYYYHHMM
GO




create   FUNCTION [dbo].[GetDDMMYYYYHHMM] (@d datetime)
RETURNS varchar (30) AS  
BEGIN 
	declare @s varchar(30)

	set @s=convert(varchar(10),@d,104)+' '+right('0'+convert(varchar(10),datepart(hour,@d)),2)+':'+right('0'+convert(varchar(10),datepart(minute,@d)),2)

	RETURN(@s)
END





GO