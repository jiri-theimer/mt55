if exists (select 1 from sysobjects where  id = object_id('j25_delete') and type = 'P')
 drop procedure j25_delete
GO





CREATE   procedure [dbo].[j25_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--j25ID
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu kategorie z tabulky j25ReportCategory
declare @ref_pid int

SELECT TOP 1 @ref_pid=x31ID from x31Report WHERE x31ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jedna �ablona sestavy nebo pluginu m� vazbu na tuto kategorii ('+dbo.GetObjectAlias('x31',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	delete from j25ReportCategory where j25ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO