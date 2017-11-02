if exists (select 1 from sysobjects where  id = object_id('j24_delete') and type = 'P')
 drop procedure j24_delete
GO






CREATE   procedure [dbo].[j24_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j24id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu nepersonálního zdroje z tabulky j24NonPersonType
declare @ref_pid int

SELECT TOP 1 @ref_pid=j23ID from j23NonPerson WHERE j24ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden nepersonální zdroj je svázaný s tímto typem ('+dbo.GetObjectAlias('j23',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j24NonPersonType where j24ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO