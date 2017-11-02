if exists (select 1 from sysobjects where  id = object_id('o23_delete') and type = 'P')
 drop procedure o23_delete
GO










CREATE   procedure [dbo].[o23_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o23ID
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu adresy z tabulky o38Address

BEGIN TRANSACTION

BEGIN TRY

	if exists(select o27ID FROM o27Attachment WHERE o23ID=@pid)
	 DELETE FROM o27Attachment WHERE o23ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid

	delete from o23Notepad where o23ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  






GO