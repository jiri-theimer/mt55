if exists (select 1 from sysobjects where  id = object_id('p28_delete') and type = 'P')
 drop procedure p28_delete
GO





CREATE   procedure [dbo].[p28_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p28id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu kontaktu z tabulky p28Contact
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p28ID_Client=@pid OR p28ID_Billing=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem nebo odbìratelem minimálnì jednoho projektu ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem v minimálnì jedné faktuøe ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem v minimálnì jedné zálohové faktuøe ('+dbo.GetObjectAlias('p90',@ref_pid)+')'



if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p28ID=@pid)
	 DELETE FROM o27Attachment WHERE p28ID=@pid

	if exists(SELECT o32ID FROM o32Contact_Medium WHERE p28ID=@pid)
	 DELETE FROM o32Contact_Medium WHERE p28ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p28ID=@pid)
	 DELETE FROM o23Notepad WHERE p28ID=@pid

	if exists(select o37ID FROM o37Contact_Address WHERE p28ID=@pid)
	 begin
	  DELETE FROM o37Contact_Address WHERE p28ID=@pid

	  DELETE FROM o38Address WHERE o38ID IN (select o38ID FROM o37Contact_Address WHERE p28ID=@pid)
	 end

	if exists(select p28ID FROM p28Contact_FreeField WHERE p28ID=@pid)
	 DELETE FROM p28Contact_FreeField WHERE p28ID=@pid

	if exists(select p30ID FROM p30Contact_Person where p28ID=@pid)
	 DELETE FROM p30Contact_Person where p28ID=@pid

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328


	DELETE FROM x90EntityLog WHERE x29ID=328 AND x90RecordPID=@pid

	delete from p28Contact WHERE p28ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO