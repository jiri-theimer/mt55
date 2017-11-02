if exists (select 1 from sysobjects where  id = object_id('j11_delete') and type = 'P')
 drop procedure j11_delete
GO



CREATE   procedure [dbo].[j11_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j11id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu týmu osob z tabulky j11Team
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE a.j11ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tento tým je obsazen pøes projektovou roli minimálnì v jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tento tým je obsazen rolí minimálnì v jednom záznamu kontaktu ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=391
 set @err_ret='Tento tým je obsazen rolí minimálnì v jedné faktuøe ('+dbo.GetObjectAlias('p91',@x69recordpid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select o20ID FROM o20Milestone_Receiver WHERE j11ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE j11ID=@pid

	DELETE FROM x69EntityRole_Assign WHERE j11ID=@pid

	if exists(select j05ID FROM j05MasterSlave WHERE j11ID_Slave=@pid)
	 DELETE FROM j05MasterSlave WHERE j11ID_Slave=@pid


	DELETE FROM j12Team_Person WHERE j11ID=@pid

	DELETE FROM j11Team WHERE j11ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO