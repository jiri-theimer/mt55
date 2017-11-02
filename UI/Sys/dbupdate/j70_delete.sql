if exists (select 1 from sysobjects where  id = object_id('j70_delete') and type = 'P')
 drop procedure j70_delete
GO





CREATE   procedure [dbo].[j70_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j70id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky j70QueryTemplate


delete from j71QueryTemplate_Item WHERE j70ID=@pid

delete from j70QueryTemplate where j70ID=@pid
















GO