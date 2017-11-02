if exists (select 1 from sysobjects where  id = object_id('j07_delete') and type = 'P')
 drop procedure j07_delete
GO




CREATE   procedure [dbo].[j07_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j07id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu pozice z tabulky j07PersonPosition
declare @ref_pid int

SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j07ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tuto pozici ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select p52ID FROM p52PriceList_Item where j07ID=@pid)
	 DELETE FROM p52PriceList_Item WHERE j07ID=@pid

	if exists(select x69ID FROM x69EntityRole_Assign where j07ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE j07ID=@pid


	delete from j07PersonPosition where j07ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO