if exists (select 1 from sysobjects where  id = object_id('p90_aftersave') and type = 'P')
 drop procedure p90_aftersave
GO








create    PROCEDURE [dbo].[p90_aftersave]
@p90id int
,@j03id_sys int

AS

if exists(select p90ID FROM p90Proforma WHERE p90ID=@p90id AND (p90Code LIKE 'TEMP%' OR p90Code IS NULL))
 exec p90_update_code @p90id,@j03id_sys

---automaticky se spouští po uložení záznamu faktury
if exists(select p99ID FROM p99Invoice_Proforma WHERE p90ID=@p90id)
 begin
  declare @p91id int

  select @p91id=p91ID FROM p99Invoice_Proforma WHERE p90ID=@p90id

  exec p91_recalc_amount @p91id
 end







GO