if exists (select 1 from sysobjects where  id = object_id('x25_delete') and type = 'P')
 drop procedure x25_delete
GO



CREATE   procedure [dbo].[x25_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x25id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu polo�ky ��seln�ku z tabulky x25EntityField_ComboValue

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x25EntityField_ComboValue where x25ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO