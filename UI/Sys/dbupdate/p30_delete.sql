if exists (select 1 from sysobjects where  id = object_id('p30_delete') and type = 'P')
 drop procedure p30_delete
GO




CREATE   procedure [dbo].[p30_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p30id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky p30Contact_Person


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p30Contact_Person WHERE p30ID=@pid

	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO