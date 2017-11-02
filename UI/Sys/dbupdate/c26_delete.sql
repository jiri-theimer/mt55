if exists (select 1 from sysobjects where  id = object_id('c26_delete') and type = 'P')
 drop procedure c26_delete
GO






CREATE   procedure [dbo].[c26_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--c26id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu dnes svátku z tabulky c21FondCalendar


BEGIN TRANSACTION

BEGIN TRY
	delete from c22FondCalendar_Date where c26ID=@pid
	
	delete from c26Holiday where c26ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO