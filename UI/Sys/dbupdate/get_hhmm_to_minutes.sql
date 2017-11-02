if exists (select 1 from sysobjects where  id = object_id('get_hhmm_to_minutes') and type = 'FN')
 drop function get_hhmm_to_minutes
GO




CREATE  FUNCTION [dbo].[get_hhmm_to_minutes] (@hhmm varchar(50))
RETURNS int AS  
BEGIN 
	declare @i int

	set @hhmm=replace(@hhmm,' ','')

	set @i=PATINDEX('%:%',@hhmm)
	
	if @i<=0
	  begin
	     if isnumeric(@hhmm)=1
	 	return(60*convert(int,@hhmm))
	     else
	        return(0)
	  end

	declare @hours int,@minutes int

	if isnumeric(left(@hhmm,@i-1))=1
	  set @hours=left(@hhmm,@i-1)
	else
	  set @hours=0
       

	if isnumeric(right(@hhmm,len(@hhmm)-@i))=1
	  set @minutes=right(@hhmm,len(@hhmm)-@i)
	else
	  set @minutes=0

	
	

	RETURN(@hours*60+@minutes)
END








GO