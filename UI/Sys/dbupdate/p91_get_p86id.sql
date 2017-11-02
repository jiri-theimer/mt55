if exists (select 1 from sysobjects where  id = object_id('p91_get_p86id') and type = 'FN')
 drop function p91_get_p86id
GO






CREATE  FUNCTION [dbo].[p91_get_p86id](@p91id int)
RETURNS int
AS
BEGIN
  ---vrací ID bankovního úètu pro fakturu @p91id

  declare @p86id int,@j27id int,@p93id int

  select @j27id=a.j27ID,@p93id=b.p93ID
  FROM p91Invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
  WHERE a.p91ID=@p91id

  RETURN(select p86ID FROM p88InvoiceHeader_BankAccount WHERE j27ID=@j27id AND p93ID=@p93id)

 
   
END


GO