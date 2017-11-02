if exists (select 1 from sysobjects where  id = object_id('get_datename') and type = 'FN')
 drop function get_datename
GO





create FUNCTION [dbo].[get_datename] (@d datetime,@langindex int)
--@langindex=0 -> èesky, 1 -> anglicky
RETURNS varchar (3) AS  
BEGIN 

	
	declare @s varchar(3)
	set @s=left(DATENAME(weekday,@d),3)
	
	
	if @langindex=0
	  begin
		if @s='Mon'
		  set @s='Po'
		if @s='Tue'
		  set @s='Ut'
		if @s='Wed'
		  set @s='St'
		if @s='Thu'
		  set @s='Ct'
		if @s='Fri'
		  set @s='Pa'
		if @s='Sat'
		  set @s='So'
		if @s='Sun'
		  set @s='Ne'
	  end

	RETURN(@s)

	
END











GO