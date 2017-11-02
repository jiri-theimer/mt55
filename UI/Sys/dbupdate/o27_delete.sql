if exists (select 1 from sysobjects where  id = object_id('o27_delete') and type = 'P')
 drop procedure o27_delete
GO






CREATE   procedure [dbo].[o27_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--o27id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu dokumentu z tabulky o27Attachment


BEGIN TRANSACTION

BEGIN TRY
	


	delete from o27Attachment where o27ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO