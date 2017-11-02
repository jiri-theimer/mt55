if exists (select 1 from sysobjects where  id = object_id('c21_delete') and type = 'P')
 drop procedure c21_delete
GO






CREATE   procedure [dbo].[c21_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--c21id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu pracovn�ho kalend��e z tabulky c21FondCalendar
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE c21ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jedna osoba m� vazbu na tento pracovn� kalend�� ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	delete from c22FondCalendar_Date where c21ID=@pid
	
	delete from c21FondCalendar where c21ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO