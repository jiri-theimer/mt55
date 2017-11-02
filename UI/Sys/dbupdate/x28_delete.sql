if exists (select 1 from sysobjects where  id = object_id('x28_delete') and type = 'P')
 drop procedure x28_delete
GO




CREATE   procedure [dbo].[x28_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x28id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu u�ivatelsk�ho pole z tabulky x28EntityField

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x28EntityField where x28ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO