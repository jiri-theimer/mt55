if exists (select 1 from sysobjects where  id = object_id('x31_delete') and type = 'P')
 drop procedure x31_delete
GO






CREATE   procedure [dbo].[x31_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x31id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu sestavy z tabulky x31Report





BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=931)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=931

	if exists(select o27ID FROM o27Attachment WHERE x31ID=@pid)
	 DELETE FROM o27Attachment WHERE x31ID=@pid


	delete from x31Report where x31ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO