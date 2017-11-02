if exists (select 1 from sysobjects where  id = object_id('x38_delete') and type = 'P')
 drop procedure x38_delete
GO









CREATE   procedure [dbo].[x38_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x38id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu èíselné øady z tabulky x38CodeLogic
declare @ref_pid int



SELECT TOP 1 @ref_pid=p42ID from p42ProjectType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ projektu je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p29ID from p29ContactType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ kontaktu je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ faktury je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p89ID from p89ProformaType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ zálohy je svázaný s touto èíselnou øadou.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x38CodeLogic where x38ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO