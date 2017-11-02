if exists (select 1 from sysobjects where  id = object_id('p91_aftersave') and type = 'P')
 drop procedure p91_aftersave
GO







CREATE    PROCEDURE [dbo].[p91_aftersave]
@p91id int
,@j03id_sys int
,@recalc_amount bit

AS

if exists(select p91ID FROM p91Invoice WHERE p91ID=@p91id AND (p91Code LIKE 'TEMP%' OR p91Code IS NULL))
 exec p91_update_code @p91id,@j03id_sys

---automaticky se spouští po uložení záznamu faktury
if @recalc_amount=1
 exec p91_recalc_amount @p91id







GO