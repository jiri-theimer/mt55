if exists (select 1 from sysobjects where  id = object_id('o32_delete') and type = 'P')
 drop procedure o32_delete
GO










CREATE   procedure [dbo].[o32_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--o32id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu kontaktn�ho m�dia z tabulky o32Contact_Medium



delete from o32Contact_Medium where o32ID=@pid








GO