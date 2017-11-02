if exists (select 1 from sysobjects where  id = object_id('p91_proforma_delete') and type = 'P')
 drop procedure p91_proforma_delete
GO






CREATE     procedure [dbo].[p91_proforma_delete]
@p91id int
,@p90id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT

AS

set @p91id=isnull(@p91id,0)
set @p90id=isnull(@p90id,0)


if @err_ret<>''
  return


delete from p99Invoice_Proforma WHERE p91ID=@p91id AND p90ID=@p90id

exec p91_recalc_amount @p91id


































GO