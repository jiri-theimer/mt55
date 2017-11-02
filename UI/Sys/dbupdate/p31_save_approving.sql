if exists (select 1 from sysobjects where  id = object_id('p31_save_approving') and type = 'P')
 drop procedure p31_save_approving
GO



CREATE  procedure [dbo].[p31_save_approving]
@p31id int
,@j03id_sys int
,@p71id int
,@p72id int
,@value_approved_internal float
,@value_approved_billing float
,@rate_billing_approved float
,@rate_internal_approved float
,@p31text nvarchar(2000)
,@vatrate_approved float
,@err_ret varchar(1000) OUTPUT

AS

set @err_ret=''
---exec p31_testbeforesave_approving @p31id,@j03id_sys,@p71id,@p72id,@hours_approved,@value_approved_billing,@amount_withoutvat_approved,@rate_billing_approved,@err_ret OUTPUT

if @err_ret<>''
 return


set @vatrate_approved=isnull(@vatrate_approved,0)

----validace sazby dph------------------
declare @vatisok bit,@p31date datetime,@p33code varchar(10),@p41id int,@j27id_orig int,@j02id_sys int,@p32id int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31date=p31date,@p33code=p33Code,@p41id=a.p41id,@j27id_orig=a.j27ID_Billing_Orig,@p32id=p32.p32ID
from p31worksheet a inner join p41project b on a.p41id=b.p41id
left outer join p32activity p32 on a.p32id=p32.p32id
left outer join p34activitygroup p34 on p32.p34id=p34.p34id
left outer join p33ActivityInputType p33 on p34.p33id=p33.p33id
where a.p31id=@p31id

if @p33code='M'
 begin
  select @vatrate_approved=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
 end

if @p33code='MV'	--DPH sazba se u schvalování testuje jenom u finanèních worksheetù
 begin
  select @vatisok=dbo.p31_testvat(@vatrate_approved,@p41id,@p31date,@j27id_orig)
  if @vatisok=0
    set @err_ret='Sazba DPH ['+convert(varchar(10),@vatrate_approved)+'%] není platná pro dané období, projekt a mìnu!'
 end
-----------------------------------------

if @err_ret<>''
  return


if @p72id=2 or @p72id=6 or @p72id=3
 begin
  set @rate_billing_approved=0		---odpis nebo paušál má fakturaèní sazbu celkovou cenu nulovou

   set @value_approved_billing=0
 end
 
if @p72id=6		--zahrnout do paušálu - schválená hodnota je pùvodní hodnota
 begin
  select @value_approved_billing=p31value_approved_billing from p31WorkSheet where p31ID=@p31id
 end 
 
if @p72id=2 or @p72id=3		--odpis nuluje schválené hodnoty
 begin
   set @value_approved_billing=0
 end 

declare @minutes int,@hours float,@hours_internal float,@minutes_internal int
set @hours=0
set @hours_internal=0
set @minutes_internal=0

if @p33code='T'
 begin
  set @hours=@value_approved_billing
  set @minutes=@value_approved_billing*60
  
  set @hours_internal=@value_approved_internal
  set @minutes_internal=@value_approved_internal*60
 end
 
 
update p31worksheet set p71id=@p71id,p72ID_AfterApprove=@p72id,j02ID_ApprovedBy=@j02id_sys,p31Approved_When=getdate()
where p31id=@p31id

if @p71id=2 or @p71id=3
  begin	---neschváleno - vynulovat hodnoty
	set @hours=0
	set @minutes=0
	set @value_approved_billing=0
	set @hours_internal=0
	set @minutes_internal=0
  end 


if @p71id is not null
 begin
   if @p33code='T'
 	update p31worksheet set p31Minutes_Approved_Billing=@minutes,p31HHMM_Approved_Billing=dbo.Minutes2HHMM(@minutes),p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@hours,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@hours*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@hours+@rate_billing_approved*@hours*@vatrate_approved/100
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @hours else 0 end
	,p31Hours_Approved_Billing=@hours,p31Hours_Approved_Internal=@hours_internal
	where p31id=@p31id

   if @p33code='U'
 	update p31worksheet set p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@value_approved_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@value_approved_billing,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@value_approved_billing+@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id =4 then @value_approved_billing else 0 end
	where p31id=@p31id

   if @p33code='M' or @p33code='MV'
   begin
 	update p31worksheet set p31Amount_WithoutVat_Approved=@value_approved_billing,p31vatrate_approved=@vatrate_approved
	,p31amount_withvat_approved=(case when p31vatrate_orig<>@vatrate_approved or @value_approved_billing<>p31amount_withoutvat_orig then @value_approved_billing+@value_approved_billing*@vatrate_approved/100 else p31amount_withvat_orig end)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @value_approved_billing else 0 end
	where p31id=@p31id
   end

   update p31worksheet set p31Amount_Vat_Approved=p31amount_withvat_approved-p31amount_withoutvat_approved
   where p31id=@p31id

   if isnull(@p31text,'')<>''
    	update p31worksheet set p31text=@p31text where p31id=@p31id
 end
else
 begin
 	update p31worksheet set p31Value_Approved_Billing=null,p31Value_Approved_Internal=null
	,p31Minutes_Approved_Billing=null,p31HHMM_Approved_Billing=null,p31Hours_Approved_Billing=null,p31Hours_Approved_Internal=null
	,p31Rate_Billing_Approved=null,p31Rate_Internal_Approved=null
 	,p31Amount_WithoutVat_Approved=null,p31Amount_WithVat_Approved=null,p31Amount_Vat_Approved=null,p31VatRate_Approved=null
 	,j02ID_ApprovedBy=null,p31approved_when=null
 	where p31id=@p31id
 end































































GO