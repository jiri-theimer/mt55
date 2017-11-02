if exists (select 1 from sysobjects where  id = object_id('p92_delete') and type = 'P')
 drop procedure p92_delete
GO





CREATE   procedure [dbo].[p92_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p92id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu typu faktury z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p92ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jedna faktura m� vazbu na tento typ ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p92InvoiceType where p92ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO