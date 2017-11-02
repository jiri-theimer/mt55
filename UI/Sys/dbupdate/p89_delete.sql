if exists (select 1 from sysobjects where  id = object_id('p89_delete') and type = 'P')
 drop procedure p89_delete
GO




CREATE   procedure [dbo].[p89_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p89id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu typu z�lohy z tabulky p89ProformaType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p89ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jedna z�lohov� faktura m� vazbu na tento typ ('+dbo.GetObjectAlias('p90',@ref_pid)+')'


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