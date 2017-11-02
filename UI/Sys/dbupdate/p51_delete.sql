if exists (select 1 from sysobjects where  id = object_id('p51_delete') and type = 'P')
 drop procedure p51_delete
GO





CREATE   procedure [dbo].[p51_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--p51id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu instituce z tabulky p51PriceList
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden klient m� vazbu na tento cen�k ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden projekt m� vazbu na tento cen�k ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	delete from p52PriceList_Item WHERE p51ID=@pid

	delete from p51PriceList where p51ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO