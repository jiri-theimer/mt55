if exists (select 1 from sysobjects where  id = object_id('x25_delete') and type = 'P')
 drop procedure x25_delete
GO



CREATE   procedure [dbo].[x25_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x25id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu položky èíselníku z tabulky x25EntityField_ComboValue

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x25EntityField_ComboValue where x25ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO