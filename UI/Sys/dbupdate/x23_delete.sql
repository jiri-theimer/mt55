if exists (select 1 from sysobjects where  id = object_id('x23_delete') and type = 'P')
 drop procedure x23_delete
GO






CREATE   procedure [dbo].[x23_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x23id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu combo ��seln�ku z tabulky x27EntityFieldGroup
if exists(select x25ID FROM x25EntityField_ComboValue WHERE x23ID=@pid)
 set @err_ret='��seln�k obsahuje minim�ln� jednu polo�ku - je t�eba vy�istit polo�ky ��seln�ku.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x23EntityField_Combo where x23ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO