if exists (select 1 from sysobjects where  id = object_id('o21_delete') and type = 'P')
 drop procedure o21_delete
GO





CREATE   procedure [dbo].[o21_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o21id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu milníku z tabulky o21MilestoneType
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o22Milestone WHERE o21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden milník/termín/událost je svázaný s tímto typem ('+dbo.GetObjectAlias('o22',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	


	delete from o21MilestoneType where o21ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO