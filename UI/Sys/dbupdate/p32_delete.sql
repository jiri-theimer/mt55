if exists (select 1 from sysobjects where  id = object_id('p32_delete') and type = 'P')
 drop procedure p32_delete
GO





CREATE   procedure [dbo].[p32_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p51PriceList
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where p32ID=@pid)
 set @err_ret='Minimálnì jeden worksheet záznam má vazbu na tuto aktivitu.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p51ID from p52PriceList_Item WHERE p32ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden ceník sazeb má vazbu na tuto aktivitu ('+dbo.GetObjectAlias('p51',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	delete from p32Activity WHERE p32ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO