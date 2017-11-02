if exists (select 1 from sysobjects where  id = object_id('c21_delete') and type = 'P')
 drop procedure c21_delete
GO






CREATE   procedure [dbo].[c21_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--c21id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu pracovního kalendáøe z tabulky c21FondCalendar
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE c21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tento pracovní kalendáø ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


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