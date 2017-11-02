if exists (select 1 from sysobjects where  id = object_id('j74_delete') and type = 'P')
 drop procedure j74_delete
GO








CREATE   procedure [dbo].[j74_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j74id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky j74SavedGridColTemplate


if exists(select j74ID FROM j74SavedGridColTemplate WHERE j74ID=@pid AND j74IsSystem=1)
 set @err_ret='Výchozí šablonu sloupcù nelze odstranit.'


if isnull(@err_ret,'')<>''
 return 

delete from j74SavedGridColTemplate where j74ID=@pid















GO