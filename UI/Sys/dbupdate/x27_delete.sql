if exists (select 1 from sysobjects where  id = object_id('x27_delete') and type = 'P')
 drop procedure x27_delete
GO










CREATE   procedure [dbo].[x27_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x27id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu skupiny uživatelských polí z tabulky x27EntityFieldGroup
if exists(select x28ID FROM x28EntityField WHERE x27ID=@pid)
 set @err_ret='Minimálnì jedno uživatelské pole je svázané s touto skupinou.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x27EntityFieldGroup where x27ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO