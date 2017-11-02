if exists (select 1 from sysobjects where  id = object_id('p40_aftersave') and type = 'P')
 drop procedure p40_aftersave
GO







CREATE PROCEDURE [dbo].[p40_aftersave]
@p40id int
,@j03id_sys int

AS

exec [p39_generate_recurrence] @p40id


declare @j02id int
select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

exec j03_recovery_cache @j03id_sys,@j02id

GO