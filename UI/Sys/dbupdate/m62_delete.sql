if exists (select 1 from sysobjects where  id = object_id('m62_delete') and type = 'P')
 drop procedure m62_delete
GO





CREATE   procedure [dbo].[m62_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--m62id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu m�nov�ho kurzu z tabulky m62ExchangeRate


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from m62ExchangeRate where m62ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO