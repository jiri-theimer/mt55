if exists (select 1 from sysobjects where  id = object_id('x55_delete') and type = 'P')
 drop procedure x55_delete
GO





CREATE   procedure [dbo].[x55_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x55id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu z tabulky x55HtmlSnippet

if exists(select x57ID FROM x57UserPageBinding WHERE x55ID=@pid)
 set @err_ret='Minimálnì jedna osobní stránka využívá tento BOX.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	
	DELETE FROM x56SnippetProperty WHERE x55ID=@pid

	delete from x55HtmlSnippet where x55ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO