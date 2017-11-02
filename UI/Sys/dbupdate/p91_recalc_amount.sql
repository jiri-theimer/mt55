if exists (select 1 from sysobjects where  id = object_id('p91_recalc_amount') and type = 'P')
 drop procedure p91_recalc_amount
GO



CREATE procedure [dbo].[p91_recalc_amount]
@p91id int

AS

declare @j27id_dest int,@datSupply datetime,@j27id_domestic int,@p92invoicetype int,@p92id int,@p41id_first int,@j17id int

select @j27id_dest=a.j27id,@datSupply=a.p91DateSupply,@p92id=a.p92id,@p92invoicetype=b.p92InvoiceType,@p41id_first=a.p41ID_First,@j17id=a.j17ID
from p91invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
where a.p91id=@p91id

declare @p41id_test int

select top 1 @p41id_test=p41id from p31worksheet where p91id=@p91id


--*****************mìnové kurzy*************************---------------
if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Domestic')
 select @j27id_domestic=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Domestic'
else
 set @j27id_domestic=2


declare @exchangedate datetime
if @j27id_domestic<>@j27id_dest
 begin
  select TOP 1 @exchangedate=m62date FROM m62ExchangeRate where j27id_master=@j27id_domestic and m62date<=@datSupply and j27id_slave=@j27id_dest order by m62date desc

  update p91invoice set p91DateExchange=@exchangedate,p91ExchangeRate=dbo.get_exchange_rate(1,p91DateSupply,j27id,@j27id_domestic)
  where p91id=@p91id
 end
else
 update p91invoice set p91DateExchange=null,p91ExchangeRate=1 where p91id=@p91id


----výchozí mìnový kurz pocházející z j27id worksheet záznamu-----------
update p31worksheet set p31ExchangeRate_Invoice=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Orig,@j27id_dest)
WHERE p91id=@p91id

----mìnový kurz pro manuálnì upravované èástky ve faktuøe-----------
update p31worksheet set p31ExchangeRate_InvoiceManual=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Invoiced,@j27id_dest)
WHERE p91id=@p91id and p31IsInvoiceManual=1


update p31worksheet set p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved*p31ExchangeRate_Invoice
,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved*p31ExchangeRate_Invoice
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved*p31ExchangeRate_Invoice
,p31rate_billing_invoiced=p31rate_billing_approved*p31ExchangeRate_Invoice
where p91id=@p91id and p31IsInvoiceManual=0

update p31worksheet set p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Invoiced*p31ExchangeRate_InvoiceManual
,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Invoiced*p31ExchangeRate_InvoiceManual
,p31Amount_Vat_Invoiced=p31Amount_Vat_Invoiced*p31ExchangeRate_InvoiceManual
,p31rate_billing_invoiced=p31rate_billing_invoiced*p31ExchangeRate_InvoiceManual
where p91id=@p91id and p31IsInvoiceManual=1

if exists(select p31id from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34activitygroup c on b.p34id=c.p34id where a.p91id=@p91id AND p31amount_withoutvat_invoiced*isnull(p31vatrate_invoiced,0)/100<>isnull(p31amount_vat_invoiced,0))
 begin  ---existují fakturované úkony, u kterých nesedí výpoèet DPH
  update p31worksheet set p31amount_vat_invoiced=p31amount_withoutvat_invoiced*p31vatrate_invoiced/100
  ,p31amount_withvat_invoiced=p31amount_withoutvat_invoiced+(p31amount_withoutvat_invoiced*p31vatrate_invoiced/100)
  WHERE p91id=@p91id
    
 end

update p31WorkSheet set p31Value_Invoiced=p31Amount_WithoutVat_Invoiced
where p91ID=@p91id and p32ID in (select p32ID from p32Activity a inner join p34ActivityGroup b on a.p34ID=b.p34ID where b.p33ID in (2,5))

update p31worksheet set j27ID_Billing_Invoiced=@j27id_dest
where p91id=@p91id and isnull(j27ID_Billing_Invoiced,0)<>@j27id_dest

----mìnový kurz z fakturaèní mìny do domácí mìny----------
update p31worksheet set j27ID_Billing_Invoiced_Domestic=@j27id_domestic, p31ExchangeRate_Domestic=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Invoiced,@j27id_domestic)
WHERE p91id=@p91id

update p31worksheet set p31Amount_WithoutVat_Invoiced_Domestic=p31Amount_WithoutVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_WithVat_Invoiced_Domestic=p31Amount_WithVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_Vat_Invoiced_Domestic=p31Amount_Vat_Invoiced*p31ExchangeRate_Domestic
where p91id=@p91id

--***************èástky DPH***********************************--
declare @p91amount_withoutvat_none float

declare @p91amount_withoutVat_low float,@p91amount_withoutVat_standard float,@p91amount_withoutVat_special float
declare @p91amount_withVat_low float,@p91amount_withVat_standard float,@p91amount_withVat_special float
declare @p91amount_Vat_low float,@p91amount_Vat_standard float,@p91amount_Vat_special float
declare @p91vatrate_low float,@p91vatrate_standard float,@p91vatrate_special float


select @p91vatrate_low=dbo.p91_get_vatrate(2,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_standard=dbo.p91_get_vatrate(3,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_special=dbo.p91_get_vatrate(4,@j27id_dest,@j17id,@datSupply)


if @p92invoicetype=2	--dobropis
 begin  ---u dobropisu brát sazby DPH z pùvodní faktury   
   select @p91vatrate_low=p91vatrate_low,@p91vatrate_standard=p91vatrate_standard,@p91vatrate_special=p91vatrate_special
   FROM
   p91Invoice WHERE p91ID in (select p91ID_CreditNoteBind FROM p91invoice where p91id=@p91id)
 end

select @p91amount_withoutvat_none=sum(p31Amount_WithoutVat_Invoiced)
FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=0

if @p91vatrate_low>0
 begin
  select @p91amount_withoutVat_low=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_low=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_low=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_low
 end

if @p91vatrate_standard>0
 begin
  select @p91amount_withoutVat_standard=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_standard=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_standard=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_standard
 end


if @p91vatrate_special>0
 begin
  select @p91amount_withoutVat_special=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_special=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_special=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_special
 end

set @p91amount_withoutvat_none=ROUND(@p91amount_withoutvat_none,2)
set @p91amount_withoutVat_low=ROUND(@p91amount_withoutVat_low,2)
set @p91amount_withVat_low=ROUND(@p91amount_withVat_low,2)
set @p91amount_Vat_low=ROUND(@p91amount_Vat_low,2)
set @p91amount_withoutVat_standard=ROUND(@p91amount_withoutVat_standard,2)
set @p91amount_withVat_standard=ROUND(@p91amount_withVat_standard,2)
set @p91amount_Vat_standard=ROUND(@p91amount_Vat_standard,2)

declare @p91amount_withoutVat float,@p91amount_withVat float,@p91amount_Vat float

set @p91amount_withoutvat_none=isnull(@p91amount_withoutvat_none,0)
set @p91amount_withoutVat_low=isnull(@p91amount_withoutVat_low,0)
set @p91amount_withoutVat_standard=isnull(@p91amount_withoutVat_standard,0)
set @p91amount_withoutVat_special=isnull(@p91amount_withoutVat_special,0)
set @p91amount_withVat_low=isnull(@p91amount_withVat_low,0)
set @p91amount_withVat_standard=isnull(@p91amount_withVat_standard,0)
set @p91amount_withVat_special=isnull(@p91amount_withVat_special,0)
set @p91amount_Vat_low=isnull(@p91amount_Vat_low,0)
set @p91amount_Vat_standard=isnull(@p91amount_Vat_standard,0)
set @p91amount_Vat_special=isnull(@p91amount_Vat_special,0)

set @p91amount_withoutVat=@p91amount_withoutvat_none+@p91amount_withoutVat_low+@p91amount_withoutVat_standard+@p91amount_withoutVat_special
set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard+@p91amount_withVat_special
set @p91amount_Vat=@p91amount_Vat_low+@p91amount_Vat_standard+@p91amount_Vat_special

--************************zaokrouhlování************************************----
declare @p97id int,@p97scale int,@p91roundfitamount float
declare @p97amountflag int	---jaká èástka je pøedmìtem zaokrouhlování: 1-èástka DPH, 2-èástka bez DPH (základ), 3-èástka vè. DPH

set @p91roundfitamount=0

if @j17id is not null
 select top 1 @p97id=p97id,@p97amountflag=p97AmountFlag,@p97scale=p97Scale from p97Invoice_Round_Setting where j17ID=@j17id and j27id=@j27id_dest

if @p97id is null
 select top 1 @p97id=p97id,@p97amountflag=p97AmountFlag,@p97scale=p97Scale from p97Invoice_Round_Setting where j17ID is null and j27id=@j27id_dest



if @p97id is not null
  begin  ----dojde k zaokrouhlování
    if @p97amountflag=3		---zaokrouhluje se celková èástka faktury
	begin
	  set @p91roundfitamount=round(@p91amount_withVat,@p97scale)-@p91amount_withVat
	  set @p91amount_withVat=@p91amount_withVat+@p91roundfitamount
	  
	end
	if @p97amountflag=2		---zaokrouhluje se celkový základ danì
	 begin
	   declare @xx float
	   set @xx=0
	   set @p91roundfitamount=0
	   
	   set @xx=round(@p91amount_withoutVat_none,@p97scale)-@p91amount_withoutVat_none
	   set @p91amount_withoutVat_none=@p91amount_withoutVat_none+@xx
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @xx=round(@p91amount_withoutVat_low,@p97scale)-@p91amount_withoutVat_low
	   set @p91amount_withoutVat_low=@p91amount_withoutVat_low+@xx
	   set @p91amount_Vat_low=@p91amount_withoutVat_low*@p91vatrate_low/100
	   set @p91amount_withVat_low=@p91amount_withoutVat_low+@p91amount_Vat_low
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @xx=round(@p91amount_withoutVat_standard,@p97scale)-@p91amount_withoutVat_standard
	   set @p91amount_withoutVat_standard=@p91amount_withoutVat_standard+@xx
	   set @p91amount_Vat_standard=@p91amount_withoutVat_standard*@p91vatrate_standard/100
	   set @p91amount_withVat_standard=@p91amount_withoutVat_standard+@p91amount_Vat_standard
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @p91amount_withoutVat=@p91amount_withoutvat_none+@p91amount_withoutVat_low+@p91amount_withoutVat_standard
	   set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard
	   set @p91amount_Vat=0+@p91amount_Vat_low+@p91amount_Vat_standard
	   --set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v èástce bez DPH
	   
	 end

    if @p97amountflag=1		---zaokrouhluje se výsledná èástka DPH
	begin
	  set @p91roundfitamount=round(@p91amount_Vat_low,@p97scale)-@p91amount_Vat_low
	  set @p91amount_Vat_low=@p91amount_Vat_low+@p91roundfitamount
	  set @p91amount_withVat_low=@p91amount_Vat_low+@p91amount_withoutVat_low
	
	  set @p91roundfitamount=round(@p91amount_Vat_standard,@p97scale)-@p91amount_Vat_standard
	  set @p91amount_Vat_standard=@p91amount_Vat_standard+@p91roundfitamount
	  set @p91amount_withVat_standard=@p91amount_Vat_standard+@p91amount_withoutVat_standard

	  set @p91roundfitamount=round(@p91amount_Vat_special,@p97scale)-@p91amount_Vat_special
	  set @p91amount_Vat_special=@p91amount_Vat_special+@p91roundfitamount
	  set @p91amount_withVat_special=@p91amount_Vat_special+@p91amount_withoutVat_special
	
	  set @p91amount_Vat=0+@p91amount_Vat_low+@p91amount_Vat_standard+@p91amount_Vat_special
	
	  set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard+@p91amount_withVat_special

	  set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v èástce DPH
	  
	end
  end


---*************- celkový dluh k zaplacení************************************
declare @p91amount_billed float,@p91amount_debt float,@datLastBilled datetime
declare @p91proformaamount float,@p91proformabilledamount float



select @p91proformaamount=sum(a.p90amount),@p91proformabilledamount=sum(a.p90Amount_Billed) FROM p90Proforma a inner join p99Invoice_Proforma b on a.p90id=b.p90id where b.p91id=@p91id

set @p91proformabilledamount=isnull(@p91proformabilledamount,0)
set @p91proformaamount=isnull(@p91proformaamount,0)

select @p91amount_billed=sum(p94Amount),@datLastBilled=max(p94Date) FROM p94Invoice_Payment where p91id=@p91id

set @p91amount_billed=isnull(@p91amount_billed,0)+@p91proformabilledamount


set @p91amount_debt=(@p91amount_withVat)-@p91amount_billed

if @p92invoicetype=2	---dobropis
 begin
  set @p91amount_debt=0
  set @p91amount_billed=@p91amount_withVat
 end

if exists(select a.p91id from p91invoice a inner join p92InvoiceType b on a.p92id=b.p92id where b.p92invoicetype=2 and a.p91ID_CreditNoteBind=@p91id)
 begin
   ---faktura je dobropisována
   declare @amount_dobropis float
   select @amount_dobropis=sum(p91amount_withVat) from p91invoice a inner join p92InvoiceType b on a.p92id=b.p92id where b.p92invoicetype=2 and a.p91ID_CreditNoteBind=@p91id
   set @p91amount_debt=@p91amount_debt-abs(@amount_dobropis)
   set @p91amount_billed=@p91amount_billed+abs(@amount_dobropis)
 end

----****závìreèný update**********************
update p91invoice set p91amount_withoutvat_none=@p91amount_withoutvat_none,p91amount_withoutVat_low=@p91amount_withoutVat_low,p91amount_withoutVat_standard=@p91amount_withoutVat_standard,p91amount_withoutVat_special=@p91amount_withoutVat_special
,p91amount_withVat_low=@p91amount_withVat_low,p91amount_withVat_standard=@p91amount_withVat_standard,p91amount_withVat_special=@p91amount_withVat_special
,p91amount_Vat_low=@p91amount_Vat_low,p91amount_Vat_standard=@p91amount_Vat_standard,p91amount_Vat_special=@p91amount_Vat_special
,p91vatrate_low=@p91vatrate_low,p91vatrate_standard=@p91vatrate_standard,p91vatrate_special=@p91vatrate_special
,p91amount_withoutVat=@p91amount_withoutVat,p91amount_withVat=@p91amount_withVat,p91amount_Vat=@p91amount_Vat
,p91amount_billed=@p91amount_billed,p91amount_debt=@p91amount_debt,p91DateBilled=@datLastBilled
,p91roundfitamount=@p91roundfitamount
,p91proformaamount=@p91proformaamount,p91proformabilledamount=@p91proformabilledamount
,p91Amount_TotalDue=@p91amount_withVat-@p91proformabilledamount
,p41id_first=@p41id_first
WHERE p91id=@p91id


---rozúètování faktury
--exec p91_invoice_statement @p91id

if @p92invoicetype=2
 begin
   --pøepoèítat ještì svázaný doklad k dobropisu
   declare @p91id_bound int
   select TOP 1 @p91id_bound=p91ID_CreditNoteBind from p91invoice where p91id=@p91id

   if @p91id_bound is not null
     exec p91_recalc_amount @p91id_bound
 end


 if exists(select x35ID FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR' AND ISNUMERIC(x35Value)=1)
  begin	---pøepoèet efektivních sazeb úkonù faktury
    declare @p51id int

	select @p51id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR'

	exec p91_fpr_recalc_invoice @p51id,@p91id
  end
  

























GO