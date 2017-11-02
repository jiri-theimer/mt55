if exists (select 1 from sysobjects where  id = object_id('p89_delete') and type = 'P')
 drop procedure p89_delete
GO




CREATE   procedure [dbo].[p89_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p89id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu zálohy z tabulky p89ProformaType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p89ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna zálohová faktura má vazbu na tento typ ('+dbo.GetObjectAlias('p90',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p89ProformaType where p89ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO