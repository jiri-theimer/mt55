if exists (select 1 from sysobjects where  id = object_id('get_hours_to_hhmm') and type = 'FN')
 drop function get_hours_to_hhmm
GO





CREATE   FUNCTION [dbo].[get_hours_to_hhmm] (@hours float)
RETURNS varchar (8) AS  
BEGIN 
	declare @bitNegative bit

	if @hours<0
	 begin
	  set @bitNegative=1
 	  set @hours=@hours*-1
	 end
	else
	 set @bitNegative=0

	declare @decMinutes decimal(11,2)
	set @decMinutes=@hours*60

	declare @str varchar(8)
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

	if @bitNegative=1
	  set @str='-'+@str

	RETURN(@str)
END





GO