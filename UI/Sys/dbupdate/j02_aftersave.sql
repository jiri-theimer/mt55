if exists (select 1 from sysobjects where  id = object_id('j02_aftersave') and type = 'P')
 drop procedure j02_aftersave
GO






CREATE    PROCEDURE [dbo].[j02_aftersave]
@j02id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu osoby

exec [x90_appendlog] 102,@j02id,@j03id_sys


exec [j02_recovery]




GO