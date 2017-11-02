if exists (select 1 from sysobjects where  id = object_id('p93_delete') and type = 'P')
 drop procedure p93_delete
GO





CREATE   procedure [dbo].[p93_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p93id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu hlavi�ky dodavatele z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE p93ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden typ faktury m� vazbu na tento z�znam.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select p88ID FROM p88InvoiceHeader_BankAccount where p93ID=@pid)
	 DELETE FROM p88InvoiceHeader_BankAccount where p93ID=@pid

	DELETE from p93InvoiceHeader where p93ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO