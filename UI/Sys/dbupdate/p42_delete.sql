if exists (select 1 from sysobjects where  id = object_id('p42_delete') and type = 'P')
 drop procedure p42_delete
GO






CREATE   procedure [dbo].[p42_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p42id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p42projecttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from  p41Project WHERE p42ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden projekt má vazbu na tento typ ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p43ProjectType_Workload WHERE p42ID=@pid

	DELETE from p42ProjectType where p42ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO