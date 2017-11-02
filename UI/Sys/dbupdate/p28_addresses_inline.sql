if exists (select 1 from sysobjects where  id = object_id('p28_addresses_inline') and type = 'FN')
 drop function p28_addresses_inline
GO


CREATE    FUNCTION [dbo].[p28_addresses_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené adresy z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+c.o36Name+' ['+isnull(o38street+', ','')+isnull(o38city+', ','')+isnull(o38zip,'')+']'
from
o38Address a INNER JOIN o37Contact_Address b ON a.o38ID=b.o38ID
INNER JOIN o36AddressType c ON b.o36ID=c.o36ID
WHERE b.p28ID=@p28id


RETURN(@s)
   
END

GO