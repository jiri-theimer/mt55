if exists (select 1 from sysobjects where  id = object_id('j05_delete') and type = 'P')
 drop procedure j05_delete
GO







CREATE   procedure [dbo].[j05_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j05id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu MASTERSLAVE z tabulky j05MasterSlave



BEGIN TRANSACTION

BEGIN TRY
	
	delete from j05MasterSlave where j05ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO