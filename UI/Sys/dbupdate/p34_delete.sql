if exists (select 1 from sysobjects where  id = object_id('p34_delete') and type = 'P')
 drop procedure p34_delete
GO






CREATE   procedure [dbo].[p34_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p34id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky p34activitygroup
declare @ref_pid int

SELECT TOP 1 @ref_pid=p32ID from p32Activity WHERE p34ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna aktivita má vazbu na tento sešit ('+dbo.GetObjectAlias('p32',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p43ProjectType_Workload where p34ID=@pid

	DELETE FROM o28ProjectRole_Workload WHERE p34ID=@pid

	delete from p34ActivityGroup where p34ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO