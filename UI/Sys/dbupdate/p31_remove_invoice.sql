if exists (select 1 from sysobjects where  id = object_id('p31_remove_invoice') and type = 'P')
 drop procedure p31_remove_invoice
GO





CREATE procedure [dbo].[p31_remove_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---vyjmutí worksheet záznamù z faktury @p91id
---vstupní úkony musí být uloženy v TEMPu - p85TempBox
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


update p31WorkSheet set p91id=null,p70id=null
,p31Rate_Billing_Invoiced=null,p31Minutes_Invoiced=null,p31Hours_Invoiced=null,p31HHMM_Invoiced=null
,p31Value_Invoiced=null,p31Amount_WithoutVat_Invoiced=null,p31Amount_WithVat_Invoiced=null
,p31Amount_Vat_Invoiced=null,p31VatRate_Invoiced=null
,p31ExchangeRate_Domestic=null,p31ExchangeRate_Invoice=null,p31ExchangeRate_InvoiceManual=null
,j27ID_Billing_Invoiced=null,j27ID_Billing_Invoiced_Domestic=null
,p31IsInvoiceManual=0,j02ID_InvoiceManual=null
where p31ID IN (select p85DataPID FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31')

exec p91_recalc_amount @p91id
































GO