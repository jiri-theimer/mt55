if exists (select 1 from sysobjects where  id = object_id('p40_delete') and type = 'P')
 drop procedure p40_delete
GO



CREATE   procedure [dbo].[p40_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p40id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu adresy z tabulky p40WorkSheet_Recurrence

BEGIN TRANSACTION

BEGIN TRY

	if exists(select p39ID FROM p39WorkSheet_Recurrence_Plan WHERE p40ID=@pid)
	 DELETE FROM p39WorkSheet_Recurrence_Plan WHERE p40ID=@pid


	delete from p40WorkSheet_Recurrence where p40ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  







GO