if exists (select 1 from sysobjects where  id = object_id('p28_medias_inline') and type = 'FN')
 drop function p28_medias_inline
GO


CREATE    FUNCTION [dbo].[p28_medias_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené kontaktní média z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+'['+b.o33Name+'] '+isnull(a.o32Value,'')+isnull(' ('+a.o32description+')','')
from
o32Contact_Medium a INNER JOIN o33MediumType b ON a.o33ID=b.o33ID
WHERE a.p28ID=@p28id

RETURN(@s)
   
END

GO