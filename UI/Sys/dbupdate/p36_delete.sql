if exists (select 1 from sysobjects where  id = object_id('p36_delete') and type = 'P')
 drop procedure p36_delete
GO






CREATE   procedure [dbo].[p36_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p36id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu  z tabulky p36LockPeriod


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p37LockPeriod_Sheet WHERE p36ID=@pid

	delete from p36LockPeriod where p36ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO