if exists (select 1 from sysobjects where  id = object_id('j17_delete') and type = 'P')
 drop procedure j17_delete
GO






CREATE   procedure [dbo].[j17_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j17id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu státu z tabulky j17Country
declare @ref_pid int

SELECT TOP 1 @ref_pid=c26ID from c26Holiday WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden den svátku je svázaný s tímto státem ('+dbo.GetObjectAlias('c26',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tento stát ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j17Country where j17ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO