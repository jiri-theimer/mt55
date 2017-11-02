if exists (select 1 from sysobjects where  id = object_id('p29_delete') and type = 'P')
 drop procedure p29_delete
GO







CREATE   procedure [dbo].[p29_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p29id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p29contacttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p29ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden kontakt má vazbu na tento typ ('+dbo.GetObjectAlias('p28',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY


	delete from p29ContactType where p29ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO