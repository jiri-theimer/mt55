if exists (select 1 from sysobjects where  id = object_id('getfieldsize') and type = 'FN')
 drop function getfieldsize
GO






CREATE FUNCTION [dbo].[getfieldsize] (@field varchar(50),@table varchar(50))
RETURNS int AS  
BEGIN 

---vrací maximální délku pole @field v tabulce @table
	
declare @id int

select @id=[id]
from sysobjects
where [name] like @table

if isnull(@id,0)=0
  return(0)

declare @size int,@type int,@xtype int

select @size=[length],@type=[type],@xtype=[xtype]
from
dbo.syscolumns
where [name] like @field and [id]=@id

if @xtype=231
  set @size=@size/2	--nvarchar typ dìlíme 2

if @type=35
  return(0)	--memo


RETURN(isnull(@size,0))


END






GO