if exists (select 1 from sysobjects where  id = object_id('Minutes2HHMM') and type = 'FN')
 drop function Minutes2HHMM
GO








CREATE FUNCTION [dbo].[Minutes2HHMM] (@decMinutes decimal (11,2))
RETURNS varchar (7) AS  
BEGIN 
	declare @str varchar(7)
	declare @intHours int
	declare @intMinutes int

	set @intHours=convert(int,@decMinutes/60)
	set @intMinutes=convert(int,@decMinutes)-@intHours*60
	
	if @intHours<10
		begin
			set @str=right('0'+convert(varchar(2),@intHours),2)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end
	else
		begin
			set @str=convert(varchar(4),@intHours)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end

	RETURN(@str)
END






GO