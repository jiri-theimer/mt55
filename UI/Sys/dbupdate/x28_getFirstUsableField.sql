if exists (select 1 from sysobjects where  id = object_id('x28_getFirstUsableField') and type = 'FN')
 drop function x28_getFirstUsableField
GO




CREATE FUNCTION [dbo].[x28_getFirstUsableField] (@entityprefix varchar(50),@x28datatype varchar(50),@x23id int)
RETURNS varchar(50) AS  
BEGIN 
	
declare @id int,@table varchar(50),@mask varchar(50),@ret varchar(50),@x29id int

select @x29id=x29id,@table=x29TableName+'_FreeField' from x29Entity where left(x29TableName,3)=@entityprefix

if @x28datatype='string'
  set @mask=@entityprefix+'freetext%'

if @x28datatype='boolean'
  set @mask=@entityprefix+'freeboolean%'

if @x28datatype='date' or @x28datatype='datetime' or @x28datatype='time'
  set @mask=@entityprefix+'freedate%'

if @x28datatype='decimal' or @x28datatype='integer'
  set @mask=@entityprefix+'freenumber%'

if isnull(@x23id,0)<>0
 set @mask=@entityprefix+'freecombo%'

select @id=[id]
from sysobjects
where [name] like @table

if isnull(@id,0)=0
  return(null)

select top 1 @ret=[name]
from
dbo.syscolumns
where [name] like @mask and [id]=@id and upper([name]) not in (select upper(x28field) from x28EntityField where x29id=@x29id and x28Field is not null)

RETURN(@ret)


END


























GO