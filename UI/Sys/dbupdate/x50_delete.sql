if exists (select 1 from sysobjects where  id = object_id('x50_delete') and type = 'P')
 drop procedure x50_delete
GO





CREATE   procedure [dbo].[x50_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x50id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu pole z tabulky x50Help

if exists(select o27ID FROM o27Attachment where x50ID=@pid)
 DELETE FROM o27Attachment where x50ID=@pid

delete from x50Help where x50ID=@pid





























GO