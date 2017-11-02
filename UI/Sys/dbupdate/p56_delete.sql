if exists (select 1 from sysobjects where  id = object_id('p56_delete') and type = 'P')
 drop procedure p56_delete
GO




CREATE   procedure [dbo].[p56_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p56id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu úkolu z tabulky p56Task
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p56ID=@pid
if @ref_pid is not null
 set @err_ret='K úkolu má vazbu minimálnì jeden worksheet úkon.'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p56ID=@pid)
	 DELETE FROM o27Attachment WHERE p56ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p56ID=@pid)
	 DELETE FROM o23Notepad WHERE p56ID=@pid

	if exists(SELECT o22ID FROM o22Milestone WHERE p56ID=@pid)
	 DELETE FROM o22Milestone WHERE p56ID=@pid

	if exists(select p56ID FROM p56Task_FreeField WHERE p56ID=@pid)
	 DELETE FROM p56Task_FreeField WHERE p56ID=@pid

	

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356

	
	DELETE FROM x90EntityLog WHERE x29ID=356 AND x90RecordPID=@pid


	delete from p56Task WHERE p56ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO