if exists (select 1 from sysobjects where  id = object_id('j23_delete') and type = 'P')
 drop procedure j23_delete
GO






CREATE   procedure [dbo].[j23_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j23id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu nepersonálního zdroje z tabulky j23NonPerson
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o19Milestone_NonPerson WHERE j23ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna rezervaèní událost má vazbu na tento zdroj ('+dbo.GetObjectAlias('o22',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j23NonPerson where j23ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO