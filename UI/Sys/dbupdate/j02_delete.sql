if exists (select 1 from sysobjects where  id = object_id('j02_delete') and type = 'P')
 drop procedure j02_delete
GO





CREATE   procedure [dbo].[j02_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu osoby z tabulky j02Person
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where j02ID=@pid)
 set @err_ret='Minimálnì jeden worksheet záznam má vazbu na tuto osobu.'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu worksheet úkonu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu projektu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu záznamu kontaktu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu záznamu faktury je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	UPDATE j03User SET j02ID=NULL WHERE j02ID=@pid

	if exists(SELECT x69ID FROM x69EntityRole_Assign WHERE j02ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE j02ID=@pid

	if exists(select o27ID FROM o27Attachment where j02ID=@pid)
	 DELETE FROM o27Attachment WHERE j02ID=@pid

	if exists(select o23ID FROM o23Notepad where j02ID=@pid)
	 DELETE FROM o23Notepad WHERE j02ID=@pid

	if exists(select j12ID FROM j12Team_Person where j02ID=@pid)
	 DELETE FROM j12Team_Person WHERE j02ID=@pid

	if exists(SELECT p30ID FROM p30Contact_Person WHERE j02ID=@pid)
	 DELETE FROM p30Contact_Person WHERE j02ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE j02ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE j02ID=@pid

	if exists(SELECT j05ID FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid)
	 DELETE FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid


	DELETE FROM x90EntityLog WHERE x29ID=102 AND x90RecordPID=@pid


	DELETE FROM j02Person WHERE j02ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO