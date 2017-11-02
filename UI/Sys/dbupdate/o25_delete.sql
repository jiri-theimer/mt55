if exists (select 1 from sysobjects where  id = object_id('o25_delete') and type = 'P')
 drop procedure o25_delete
GO











CREATE   procedure [dbo].[o25_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o25id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu z tabulky o25DmsBinding

BEGIN TRANSACTION

BEGIN TRY

	

	delete from o25DmsBinding where o25ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  







GO