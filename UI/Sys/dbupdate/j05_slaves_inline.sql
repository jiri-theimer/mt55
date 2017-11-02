if exists (select 1 from sysobjects where  id = object_id('j05_slaves_inline') and type = 'FN')
 drop function j05_slaves_inline
GO


CREATE    FUNCTION [dbo].[j05_slaves_inline](@j02id_master int)
RETURNS varchar(6000)
AS
BEGIN
  ---vrací èárkou oddìlené názvy podøízených osob a týmù

 DECLARE @s varchar(6000)

select @s=COALESCE(@s + ', ', '')+a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')
  FROM j02Person a INNER JOIN j05MasterSlave b ON a.j02ID=b.j02ID_Slave
  WHERE b.j02ID_Master=@j02id_master
  ORDER BY a.j02LastName


RETURN(@s)
   
END



GO