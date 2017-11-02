if exists (select 1 from sysobjects where  id = object_id('o21_delete') and type = 'P')
 drop procedure o21_delete
GO





CREATE   procedure [dbo].[o21_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--o21id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu typu miln�ku z tabulky o21MilestoneType
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o22Milestone WHERE o21ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden miln�k/term�n/ud�lost je sv�zan� s t�mto typem ('+dbo.GetObjectAlias('o22',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	


	delete from o21MilestoneType where o21ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO