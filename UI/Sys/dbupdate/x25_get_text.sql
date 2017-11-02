if exists (select 1 from sysobjects where  id = object_id('x25_get_text') and type = 'FN')
 drop function x25_get_text
GO


CREATE FUNCTION [dbo].[x25_get_text](@x23id int,@x25id int)
RETURNS nvarchar(255) AS  
BEGIN 

if ISNULL(@x25id,0)=0
 RETURN(NULL)

declare @ret nvarchar(255)

select @ret=x25Name
from
x25EntityField_ComboValue
where x23ID=@x23id AND x25ID=@x25id


RETURN(@ret)

END



GO