if exists (select 1 from sysobjects where  id = object_id('p41_delete') and type = 'P')
 drop procedure p41_delete
GO






CREATE   procedure [dbo].[p41_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p41id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu projektu z tabulky p41Project
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='Do projektu byl zapsán minimálnì jeden worksheet úkon.'


set @ref_pid=null
SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='K projektu je vytvoøen minimálnì jeden úkol ('+dbo.GetObjectAlias('p56',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p41ID=@pid)
	 DELETE FROM o27Attachment WHERE p41ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p41ID=@pid)
	 DELETE FROM o23Notepad WHERE p41ID=@pid

	if exists(SELECT o22ID FROM o22Milestone WHERE p41ID=@pid)
	 DELETE FROM o22Milestone WHERE p41ID=@pid

	if exists(select p41ID FROM p41Project_FreeField WHERE p41ID=@pid)
	 DELETE FROM p41Project_FreeField WHERE p41ID=@pid

	if exists(select o39ID FROM o39Project_Address WHERE p41ID=@pid)
	 DELETE FROM o39Project_Address WHERE p41ID=@pid

	if exists(select p30ID FROM p30Contact_Person WHERE p41ID=@pid)
	 DELETE FROM p30Contact_Person WHERE p41ID=@pid

	if exists(select p47ID FROM p47CapacityPlan WHERE p41ID=@pid)
	 DELETE FROM p47CapacityPlan WHERE p41ID=@pid

	if exists(select p48ID FROM p48OperativePlan WHERE p41ID=@pid)
	 DELETE FROM p48OperativePlan WHERE p41ID=@pid

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141

	
	DELETE FROM x90EntityLog WHERE x29ID=141 AND x90RecordPID=@pid


	delete from p41Project WHERE p41ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO