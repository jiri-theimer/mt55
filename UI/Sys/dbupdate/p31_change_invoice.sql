if exists (select 1 from sysobjects where  id = object_id('p31_change_invoice') and type = 'P')
 drop procedure p31_change_invoice
GO




CREATE procedure [dbo].[p31_change_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---zmìna vyfakturovaných úkonù ve faktuøe @p91id
---vstupní úkony musí být již obsaženy ve faktuøe a uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID
---p70id - p85OtherKey1
---p31Text - p85Message
---p31Value_Invoiced - p85FreeFloat01  (èástka bez DPH u penìz nebo hodiny u èasu)
---p31Rate_Billing_Invoiced - p85FreeFloat02 (hodinová nebo úkonová sazba)
---p31VatRate_Invoiced - p85FreeFloat03  (explicitní sazba DPH)

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')



if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if isnull(@p91id,0)=0
  set @err_ret='Chybí faktura @p91id!'

if exists(select p85ID FROM p85TempBox WHERE p85GUID=@guid AND (p85OtherKey1 IS NULL OR p85DataPID IS NULL))
 set @err_ret='TEMP data p85GUID or p85OtherKey1 missing.'

if @err_ret<>''
 return

declare @login nvarchar(50),@j02id_sys int

select @j02id_sys=j02ID,@login=j03Login FROM j03User WHERE j03ID=@j03id_sys


declare @j27id int,@x15id int,@p91fixedvatrate float


select @j27id=j27ID,@x15id=x15ID,@p91fixedvatrate=p91FixedVatRate
from p91Invoice
where p91ID=@p91id  

declare @p31id int,@p70id_edit int,@vatrate_edit float,@value_edit float,@text_edit nvarchar(2000),@rate_edit float
declare @p33id int
declare @p31amount_withoutvat_invoiced float,@p31amount_vat_invoiced float,@p31amount_withvat_invoiced float

DECLARE curP31 CURSOR FOR 
select p85DataPID,p85OtherKey1,p85Message,p85FreeFloat01,p85FreeFloat02,p85FreeFloat03 from p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31'
OPEN curP31
FETCH NEXT FROM curP31  INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit
WHILE @@FETCH_STATUS = 0
BEGIN

 select @p33id=c.p33ID FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
 WHERE a.p31ID=@p31id


if @x15id is not null and @vatrate_edit is null
  set @vatrate_edit=@p91fixedvatrate	---DPH se pøebírá z jednotné (fixní) dph faktury
 

 if @p70id_edit=2 or @p70id_edit=3 or @p70id_edit=6	---odpis nebo paušál
  begin
   set @value_edit=0
   set @rate_edit=0
  end

 if @p33id=1	---èas
  begin
    UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Rate_Billing_Invoiced=@rate_edit
	,p31Hours_Invoiced=@value_edit,p31Minutes_Invoiced=@value_edit*60,p31HHMM_Invoiced=dbo.get_hours_to_hhmm(@value_edit)
	WHERE p31ID=@p31id
  end

 if @p33id=3	---kusovník
  begin
    UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Rate_Billing_Invoiced=@rate_edit
	WHERE p31ID=@p31id
  end
 
 if @p33id=2 or @p33id=5
  begin
   UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Amount_WithoutVat_Invoiced=@value_edit
   WHERE p31ID=@p31id
  end

 if @p33id=1 OR @p33id=3
  begin
   set @p31amount_withoutvat_invoiced=@rate_edit*@value_edit

   if @x15id is null
	 select @vatrate_edit=p31VatRate_Approved FROM p31Worksheet WHERE p31ID=@p31id	---pokud DPH není ve faktuøe fixní, pak to brát z úkonu
  end
  
  
  if @p33id=2 or @p33id=5
   set @p31amount_withoutvat_invoiced=@value_edit


  set @p31amount_vat_invoiced=@p31amount_withoutvat_invoiced*@vatrate_edit/100
  set @p31amount_withvat_invoiced=@p31amount_withoutvat_invoiced+@p31amount_vat_invoiced

  UPDATE p31Worksheet set p70ID=@p70id_edit,p31IsInvoiceManual=1,j02ID_InvoiceManual=@j02id_sys,p31DateUpdate_InvoiceManual=getdate()
  ,p31Amount_WithoutVat_Invoiced=@p31amount_withoutvat_invoiced,p31Amount_WithVat_Invoiced=@p31amount_withvat_invoiced
  ,p31Amount_Vat_Invoiced=@p31amount_vat_invoiced,p31VatRate_Invoiced=@vatrate_edit,j27ID_Billing_Invoiced=@j27id
  WHERE p31ID=@p31id

  if @text_edit is not null
   UPDATE p31Worksheet set p31Text=@text_edit WHERE p31ID=@p31id

FETCH NEXT FROM curP31 INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit
END

CLOSE curP31
DEALLOCATE curP31





exec p91_recalc_amount @p91id































GO