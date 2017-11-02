if exists (select 1 from sysobjects where  id = object_id('p48_delete') and type = 'P')
 drop procedure p48_delete
GO







CREATE   procedure [dbo].[p48_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p48id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu operativního plánu z tabulky p48OperativePlan


BEGIN TRANSACTION

BEGIN TRY

	delete from p48OperativePlan WHERE p48ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO