if exists (select 1 from sysobjects where  id = object_id('p31_append_invoice') and type = 'P')
 drop procedure p31_append_invoice
GO




CREATE procedure [dbo].[p31_append_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---vložení schválených worksheet záznamù do uložené faktury @p91id
---vstupní úkony musí být schváleny a uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')



if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if @p91id=0
  set @err_ret='Chybí faktura @p91id!'

if @err_ret<>''
 return

declare @login nvarchar(50)
set @login=dbo.j03_getlogin(@j03id_sys)


declare @j27id int,@x15id int,@p91fixedvatrate float


select @j27id=j27ID,@x15id=x15ID,@p91fixedvatrate=p91FixedVatRate
from p91Invoice
where p91ID=@p91id  




update p31worksheet set p91ID=@p91id,p70id=p72ID_AfterApprove
WHERE p91ID is null AND p71id=1 and isnull(p72ID_AfterApprove,0)<>0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p91ID=@p91id,p70id=(case when isnull(p31amount_withoutvat_approved,0)<>0 then 4 else 2 end)
WHERE p91ID is null AND p71id=1 and isnull(p72ID_AfterApprove,0)=0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p31Minutes_Invoiced=p31Minutes_Approved_Billing,p31Hours_Invoiced=p31Hours_Approved_Billing,p31HHMM_Invoiced=p31HHMM_Approved_Billing
,p31Value_Invoiced=p31Value_Approved_Billing
,p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved,p31VatRate_Invoiced=case when @x15id is not null then @p91fixedvatrate else p31VatRate_Approved end
,p31Rate_Billing_Invoiced=p31Rate_Billing_Approved
where p91ID=@p91id AND p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')



exec p91_recalc_amount @p91id































GO