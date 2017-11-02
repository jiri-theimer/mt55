if exists (select 1 from sysobjects where  id = object_id('j03_delete') and type = 'P')
 drop procedure j03_delete
GO





CREATE   procedure [dbo].[j03_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j03id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu uživatele z tabulky j03User


BEGIN TRANSACTION

BEGIN TRY

	if exists(select j90ID FROM j90LoginAccessLog where j03ID=@pid)
      DELETE FROM j90LoginAccessLog where j03ID=@pid 

	if exists(select j74ID FROM j74SavedGridColTemplate where j03ID=@pid)
	 DELETE FROM j74SavedGridColTemplate WHERE j03ID=@pid

	if exists(select j70ID FROM j70QueryTemplate where j03ID=@pid)
	 BEGIN
		DELETE FROM j71QueryTemplate_Item WHERE j70ID IN (select j70ID FROM j70QueryTemplate where j03ID=@pid)

		DELETE FROM j70QueryTemplate WHERE j03ID=@pid
	 END

	DELETE FROM x36UserParam WHERE j03ID=@pid
	 

	if exists(select x47ID FROM x47EventLog where j03ID=@pid)
      DELETE FROM x47EventLog where j03ID=@pid 

	

	delete from j03User where j03ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO