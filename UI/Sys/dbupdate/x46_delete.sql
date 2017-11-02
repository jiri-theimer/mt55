if exists (select 1 from sysobjects where  id = object_id('x46_delete') and type = 'P')
 drop procedure x46_delete
GO







CREATE   procedure [dbo].[x46_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x46id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu notifika�n�ho nastaven� z tabulky x46EventNotification


BEGIN TRANSACTION

BEGIN TRY

	delete from x46EventNotification where x46ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO