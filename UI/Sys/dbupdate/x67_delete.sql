if exists (select 1 from sysobjects where  id = object_id('x67_delete') and type = 'P')
 drop procedure x67_delete
GO






CREATE   procedure [dbo].[x67_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x67id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu role z tabulky x67EntityRole
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE b.x67ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tato role je již obsazena v minimálnì jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tato role je již obsazena v minimálnì jednom záznamu kontaktu ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=356
 set @err_ret='Tato role je již obsazena v minimálnì jedné úloze ('+dbo.GetObjectAlias('p56',@x69recordpid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select x67id from x67EntityRole where x67ID=@pid and x29ID=141)
	 begin
		--odstranit navíc naklonovanou roli pro entitu j18Region (projektová skupina)
		declare @x67id_cloned int
		
		select @x67id_cloned=x67ID FROM x67EntityRole WHERE x67ParentID=@pid

		delete from x68EntityRole_Permission WHERE x67ID=@x67id_cloned

		if exists(select o28ID FROM o28ProjectRole_Workload WHERE x67ID=@x67id_cloned)
		  DELETE FROM o28ProjectRole_Workload WHERE x67ID=@x67id_cloned

		delete from x67EntityRole where x67ID=@x67id_cloned
	 end

	delete from x68EntityRole_Permission WHERE x67ID=@pid

	if exists(select o28ID FROM o28ProjectRole_Workload WHERE x67ID=@pid)
	 DELETE FROM o28ProjectRole_Workload WHERE x67ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE x67ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE x67ID=@pid

	if exists(select x69ID FROM x69EntityRole_Assign WHERE x67ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE x67ID=@pid

	delete from x67EntityRole where x67ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO