if exists (select 1 from sysobjects where  id = object_id('p57_delete') and type = 'P')
 drop procedure p57_delete
GO






CREATE   procedure [dbo].[p57_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p57id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu instituce z tabulky p57TaskType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p56ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden �kol m� vazbu na tento typ ('+dbo.GetObjectAlias('p56',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p57TaskType where p57ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO