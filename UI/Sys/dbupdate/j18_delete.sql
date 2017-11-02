if exists (select 1 from sysobjects where  id = object_id('j18_delete') and type = 'P')
 drop procedure j18_delete
GO




CREATE   procedure [dbo].[j18_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--j18id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu regionu z tabulky j18Region
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j18ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden projekt je sv�zan� s t�mto regionem ('+dbo.GetObjectAlias('p41',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j18Region where j18ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO