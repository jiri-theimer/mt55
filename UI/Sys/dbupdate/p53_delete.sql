if exists (select 1 from sysobjects where  id = object_id('p53_delete') and type = 'P')
 drop procedure p53_delete
GO





CREATE   procedure [dbo].[p53_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p53id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu dph sazby z tabulky p53VatRate


BEGIN TRANSACTION

BEGIN TRY

	delete from p53VatRate WHERE p53ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO